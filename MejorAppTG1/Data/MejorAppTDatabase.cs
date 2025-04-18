using SQLite;
using System;
using System.IO;
using System.Threading.Tasks;
using MejorAppTG1.Models;

public class MejorAppTDatabase
{
    private readonly SQLiteAsyncConnection _database;

    public MejorAppTDatabase(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);
        _database.CreateTableAsync<Test>().Wait();
        _database.CreateTableAsync<Answer>().Wait();
        _database.CreateTableAsync<User>().Wait();
    }

    // MÉTODOS PARA TABLA TESTS

    public async Task<int> AddTestAsync(Test test)
    {
        await _database.InsertAsync(test);
        return await _database.ExecuteScalarAsync<int>("SELECT last_insert_rowid()");
    }

    public Task<List<Test>> GetTestsAsync()
    {
        return _database.Table<Test>().ToListAsync();
    }

    public Task<int> UpdateTestAsync(Test test)
    {
        return _database.UpdateAsync(test);
    }

    public Task<int> DeleteTestAsync(Test test)
    {
        return _database.DeleteAsync(test);
    }

    public async Task<int> DeleteTestAndAnswersAsync(Test test)
    {
        foreach (var answer in await GetAnswersByTestIdAsync(test.IdTest)) {
            await DeleteAnswerAsync(answer);
        }
        return await DeleteTestAsync(test);
    }

    public async Task DeleteTestsByUserAsync(int userId)
    {
        var testsToDelete = await _database.Table<Test>()
            .Where(t => t.IdUser == userId && t.IdFirebase != null)
            .ToListAsync();

        foreach (var test in testsToDelete) {
            await DeleteTestAndAnswersAsync(test);
        }
    }

    public async Task DeleteSyncedTestsWithDeletedUserInLocal()
    {
        var allUserIds = await GetUsuarioIdsAsync();
        var testsToDelete = await _database.Table<Test>()
            .Where(t => !allUserIds.Contains(t.IdUser) && !string.IsNullOrWhiteSpace(t.IdFirebase)) // Borra todos los tests que estén sincronizados y cuyo usuario haya sido eliminado
            .ToListAsync();

        foreach (var test in testsToDelete) {
            await DeleteTestAndAnswersAsync(test);
        }
    }

    public Task<Test> GetUnfinishedTestAsync(int userId, string tipoTest)
    {
        return _database.Table<Test>()
            .Where(t => t.IdUser == userId && t.Tipo == tipoTest && !t.Terminado)   // ¿Tiene este usuario algún test de ese tipo sin terminar?
            .FirstOrDefaultAsync();
    }

    public Task<List<Test>> GetAllFinishedUnsyncedTestsAsync()
    {
        return _database.Table<Test>()
            .Where(t => t.Terminado && string.IsNullOrEmpty(t.IdFirebase))
            .ToListAsync();
    }

    public Task<List<Test>> GetFinishedTestsByUserAsync(int userId)
    {
        return _database.Table<Test>()
            .Where(t => t.IdUser == userId && t.Terminado)
            .OrderByDescending(x => x.Fecha)
            .ToListAsync();
    }

    public Task<List<Test>> GetFinishedTestsByUserFilteredAsync(int userId, string tipo)
    {
        return _database.Table<Test>()
            .Where(t => t.IdUser == userId && t.Terminado && t.Tipo.Equals(tipo))
            .OrderByDescending(x => x.Fecha)
            .ToListAsync();
    }

    public async Task<Test> GetTestByIdAsync(int testId)
    {
        return await _database.Table<Test>()
            .Where(t => t.IdTest == testId)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Test>> GetTestsByUserAsync(int userId)
    {
        return await _database.Table<Test>()
            .Where(t => t.IdUser == userId)
            .ToListAsync();
    }

    // MÉTODOS PARA TABLA RESPUESTAS

    public async Task<int> AddAnswerAsync(Answer answer)
    {
        await _database.InsertAsync(answer);
        return await _database.ExecuteScalarAsync<int>("SELECT last_insert_rowid()");
    }

    public Task<List<Answer>> GetAnswersAsync()
    {
        return _database.Table<Answer>().ToListAsync();
    }

    public Task<int> UpdateAnswerAsync(Answer answer)
    {
        return _database.UpdateAsync(answer);
    }

    public Task<int> DeleteAnswerAsync(Answer answer)
    {
        return _database.DeleteAsync(answer);
    }

    public Task<List<Answer>> GetAnswersByTestIdAsync(int idTest)
    {
        return _database.Table<Answer>()
            .Where(a => a.IdTest == idTest)
            .ToListAsync();
    }

    public async Task UpdateOrInsertAnswerAsync(Answer answer)
    {
        // Buscar si ya existe una respuesta con ese IdTest e IdPregunta
        var existingAnswer = await _database.Table<Answer>()
            .Where(a => a.IdTest == answer.IdTest && a.IdPregunta == answer.IdPregunta)
            .FirstOrDefaultAsync();

        if (existingAnswer != null) {
            // Si ya existe, actualízala
            existingAnswer.Factor = answer.Factor;
            existingAnswer.ValorRespuesta = answer.ValorRespuesta;
            await _database.UpdateAsync(existingAnswer);
        }
        else {
            // Si no existe, añádela
            await _database.InsertAsync(answer);
        }
    }

    // MÉTODOS PARA TABLA USUARIOS
    public Task<List<User>> GetUsuariosAsync()
    {
        return _database.Table<User>().ToListAsync();
    }
    public async Task<List<int>> GetUsuarioIdsAsync()
    {
        var usuarios = await GetUsuariosAsync();
        return usuarios.Select(u => u.IdUsuario).ToList();
    }

    public async Task<int> AddUsuarioAsync(User user)
    {
        await _database.InsertAsync(user);
        return await _database.ExecuteScalarAsync<int>("SELECT last_insert_rowid()");
    }

    public Task<int> UpdateUsuarioAsync(User user)
    {
        return _database.UpdateAsync(user);
    }

    public Task<int> DeleteUsuarioAsync(User user)
    {
        return _database.DeleteAsync(user);
    }

    public async Task<User> GetUserByIdAsync(int userId)
    {
        return await _database.Table<User>()
            .Where(t => t.IdUsuario == userId)
            .FirstOrDefaultAsync();
    }

}
