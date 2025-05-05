using MejorAppTG1.Data;

namespace MejorAppTG1.Utils;

/// <summary>
/// Clase que alberga todos los métodos relativos a la sincronización de SQLite con Firebase.
/// </summary>
public class SyncService
{
    private readonly FirebaseService _firebaseService;
    private readonly MejorAppTDatabase _sqliteService;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="SyncService"/>.
    /// </summary>
    /// <param name="firebaseService">La base de datos de Firebase.</param>
    /// <param name="sqliteService">La base de datos de SQLite.</param>
    public SyncService(FirebaseService firebaseService, MejorAppTDatabase sqliteService)
    {
        _firebaseService = firebaseService;
        _sqliteService = sqliteService;
    }

    /// <summary>
    /// Inicia el proceso de sincronización de tests con Firebase.
    /// </summary>
    public async Task SyncTests()
    {
        var tests = await _sqliteService.GetAllFinishedUnsyncedTestsAsync();
        foreach (var test in tests) {
            var firebaseId = await _firebaseService.AddOrUpdateTest(test);
            test.IdFirebase = firebaseId;
            await _sqliteService.UpdateTestAsync(test);
        }
    }
}
