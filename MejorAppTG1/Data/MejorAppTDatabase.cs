using SQLite;
using System;
using System.IO;
using System.Threading.Tasks;
using MejorAppTG1.Models;

public class MejorAppTDatabase
{
    private readonly SQLiteAsyncConnection _database;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="MejorAppTDatabase"/>.
    /// </summary>
    /// <param name="dbPath">La ruta local de la base de datos.</param>
    public MejorAppTDatabase(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);
        _database.CreateTableAsync<Test>().Wait();
        _database.CreateTableAsync<Answer>().Wait();
        _database.CreateTableAsync<User>().Wait();
    }

    #region Tests   
    /// <summary>
    /// Añade un test a la base de datos.
    /// </summary>
    /// <param name="test">El test a añadir.</param>
    /// <returns>La clave primaria (ID) del test recién añadido.</returns>
    public async Task<int> AddTestAsync(Test test)
    {
        await _database.InsertAsync(test);
        return await _database.ExecuteScalarAsync<int>("SELECT last_insert_rowid()");
    }

    /// <summary>
    /// Devuelve todos los tests almacenados en la base de datos.
    /// </summary>
    /// <returns>Una lista con todos los tests almacenados.</returns>
    public Task<List<Test>> GetTestsAsync()
    {
        return _database.Table<Test>().ToListAsync();
    }

    /// <summary>
    /// Actualiza un test de la base de datos.
    /// </summary>
    /// <param name="test">El test a modificar.</param>
    /// <returns>La cantidad de registros actualizados.</returns>
    public Task<int> UpdateTestAsync(Test test)
    {
        return _database.UpdateAsync(test);
    }

    /// <summary>
    /// Elimina un test de la base de datos.
    /// </summary>
    /// <param name="test">El test a eliminar.</param>
    /// <returns>La cantidad de registros eliminados.</returns>
    public Task<int> DeleteTestAsync(Test test)
    {
        return _database.DeleteAsync(test);
    }

    /// <summary>
    /// Elimina un test y todas las respuestas del usuario a dicho test de la base de datos.
    /// </summary>
    /// <param name="test">El test a eliminar.</param>
    /// <returns>La cantidad de registros eliminados.</returns>
    public async Task<int> DeleteTestAndAnswersAsync(Test test)
    {
        foreach (var answer in await GetAnswersByTestIdAsync(test.IdTest)) {
            await DeleteAnswerAsync(answer);
        }
        return await DeleteTestAsync(test);
    }

    /// <summary>
    /// Elimina todos los tests y todas las respuestas de un usuario determinado de la base de datos.
    /// </summary>
    /// <param name="userId">El identificador del usuario.</param>
    public async Task DeleteTestsByUserAsync(int userId)
    {
        var testsToDelete = await _database.Table<Test>()
            .Where(t => t.IdUser == userId && t.IdFirebase != null)
            .ToListAsync();

        foreach (var test in testsToDelete) {
            await DeleteTestAndAnswersAsync(test);
        }
    }

    /// <summary>
    /// Elimina todos los tests que estén sincronizados y cuyo usuario haya sido eliminado de la base de datos.
    /// </summary>
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

    /// <summary>
    /// Si un usuario dado inició un test de un tipo dado y lo dejó a medias, este método lo recupera.
    /// </summary>
    /// <param name="userId">El identificador del usuario.</param>
    /// <param name="tipoTest">La clave del tipo de test.</param>
    /// <returns>El test que coincida con los parámetros o null si no hay tests a medias.</returns>
    public Task<Test> GetUnfinishedTestAsync(int userId, string tipoTest)
    {
        return _database.Table<Test>()
            .Where(t => t.IdUser == userId && t.Tipo == tipoTest && !t.Terminado)   // ¿Tiene este usuario algún test de ese tipo sin terminar?
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Devuelve todos los tests terminados pero no sincronizados aún con Firebase.
    /// </summary>
    /// <returns>Una lista con todos los tests no sincronizados.</returns>
    public Task<List<Test>> GetAllFinishedUnsyncedTestsAsync()
    {
        return _database.Table<Test>()
            .Where(t => t.Terminado && string.IsNullOrEmpty(t.IdFirebase))
            .ToListAsync();
    }

    /// <summary>
    /// Devuelve todos los tests que ha realizado y finalizado un usuario dado ordenados por fecha más reciente.
    /// </summary>
    /// <param name="userId">El identificador del usuario.</param>
    /// <returns>Una lista con todos los tests realizados ordenados por fecha.</returns>
    public Task<List<Test>> GetFinishedTestsByUserAsync(int userId)
    {
        return _database.Table<Test>()
            .Where(t => t.IdUser == userId && t.Terminado)
            .OrderByDescending(x => x.Fecha)
            .ToListAsync();
    }

    /// <summary>
    /// Devuelve todos los tests de un tipo dado que ha realizado y finalizado un usuario dado.
    /// </summary>
    /// <param name="userId">El identificador del usuario.</param>
    /// <param name="tipo">La clave del tipo de test.</param>
    /// <returns>Una lista con todos los tests realizados de ese tipo.</returns>
    public Task<List<Test>> GetFinishedTestsByUserFilteredAsync(int userId, string tipo)
    {
        return _database.Table<Test>()
            .Where(t => t.IdUser == userId && t.Terminado && t.Tipo.Equals(tipo))
            .ToListAsync();
    }

    /// <summary>
    /// Devuelve un test concreto por su identificador.
    /// </summary>
    /// <param name="testId">El identificador del test.</param>
    /// <returns>El test encontrado, o null si no existe.</returns>
    public async Task<Test> GetTestByIdAsync(int testId)
    {
        return await _database.Table<Test>()
            .Where(t => t.IdTest == testId)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Devuelve todos los tests asociados a un usuario, terminados o no.
    /// </summary>
    /// <param name="userId">El identificador del usuario.</param>
    /// <returns>Una lista con todos los tests del usuario.</returns>
    public async Task<List<Test>> GetTestsByUserAsync(int userId)
    {
        return await _database.Table<Test>()
            .Where(t => t.IdUser == userId)
            .ToListAsync();
    }
    #endregion

    #region Respuestas
    /// <summary>
    /// Añade una respuesta a la base de datos.
    /// </summary>
    /// <param name="answer">La respuesta a añadir.</param>
    /// <returns>La clave primaria (ID) de la respuesta recién añadida.</returns>
    public async Task<int> AddAnswerAsync(Answer answer)
    {
        await _database.InsertAsync(answer);
        return await _database.ExecuteScalarAsync<int>("SELECT last_insert_rowid()");
    }

    /// <summary>
    /// Devuelve todas las respuestas almacenadas en la base de datos.
    /// </summary>
    /// <returns>Una lista con todas las respuestas almacenadas.</returns>
    public Task<List<Answer>> GetAnswersAsync()
    {
        return _database.Table<Answer>().ToListAsync();
    }

    /// <summary>
    /// Actualiza una respuesta de la base de datos.
    /// </summary>
    /// <param name="answer">La respuesta a modificar.</param>
    /// <returns>La cantidad de registros actualizados.</returns>
    public Task<int> UpdateAnswerAsync(Answer answer)
    {
        return _database.UpdateAsync(answer);
    }

    /// <summary>
    /// Elimina una respuesta de la base de datos.
    /// </summary>
    /// <param name="answer">La respuesta a eliminar.</param>
    /// <returns>La cantidad de registros eliminados.</returns>
    public Task<int> DeleteAnswerAsync(Answer answer)
    {
        return _database.DeleteAsync(answer);
    }

    /// <summary>
    /// Devuelve todas las respuestas asociadas a un test concreto por el identificador del test.
    /// </summary>
    /// <param name="idTest">El identificador del test.</param>
    /// <returns>Las respuestas del test encontrado.</returns>
    public Task<List<Answer>> GetAnswersByTestIdAsync(int idTest)
    {
        return _database.Table<Answer>()
            .Where(a => a.IdTest == idTest)
            .ToListAsync();
    }

    /// <summary>
    /// Busca una respuesta del test en curso. Si existe, la actualiza. Si no, la añade al test.
    /// </summary>
    /// <param name="answer">La respuesta.</param>
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
    #endregion

    #region Usuarios

    /// <summary>
    /// Devuelve todos los usuarios almacenados en la base de datos.
    /// </summary>
    /// <returns>Una lista con todos los usuarios almacenados.</returns>
    public Task<List<User>> GetUsuariosAsync()
    {
        return _database.Table<User>().ToListAsync();
    }

    /// <summary>
    /// Devuelve los identificadores de todos los usuarios almacenados en la base de datos.
    /// </summary>
    /// <returns>Una lista con los identificadores de todos los usuarios almacenados.</returns>
    public async Task<List<int>> GetUsuarioIdsAsync()
    {
        var usuarios = await GetUsuariosAsync();
        return usuarios.Select(u => u.IdUsuario).ToList();
    }

    /// <summary>
    /// Añade un usuario a la base de datos.
    /// </summary>
    /// <param name="user">El usuario a añadir.</param>
    /// <returns>La clave primaria (ID) del usuario recién añadido.</returns>
    public async Task<int> AddUsuarioAsync(User user)
    {
        await _database.InsertAsync(user);
        return await _database.ExecuteScalarAsync<int>("SELECT last_insert_rowid()");
    }

    /// <summary>
    /// Actualiza un usuario de la base de datos.
    /// </summary>
    /// <param name="user">El usuario a modificar.</param>
    /// <returns>La cantidad de registros actualizados.</returns>
    public Task<int> UpdateUsuarioAsync(User user)
    {
        return _database.UpdateAsync(user);
    }

    /// <summary>
    /// Elimina un usuario de la base de datos.
    /// </summary>
    /// <param name="user">El usuario a eliminar.</param>
    /// <returns>La cantidad de registros eliminados.</returns>
    public Task<int> DeleteUsuarioAsync(User user)
    {
        return _database.DeleteAsync(user);
    }

    /// <summary>
    /// Devuelve un usuario concreto por su identificador.
    /// </summary>
    /// <param name="userId">El identificador del usuario.</param>
    /// <returns>El usuario encontrado, o null si no existe.</returns>
    public async Task<User> GetUserByIdAsync(int userId)
    {
        return await _database.Table<User>()
            .Where(t => t.IdUsuario == userId)
            .FirstOrDefaultAsync();
    }
    #endregion
}
