using MejorAppTG1.Data;

public class SyncService
{
    private readonly FirebaseService _firebaseService;
    private readonly MejorAppTDatabase _sqliteService;

    public SyncService(FirebaseService firebaseService, MejorAppTDatabase sqliteService)
    {
        _firebaseService = firebaseService;
        _sqliteService = sqliteService;
    }

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
