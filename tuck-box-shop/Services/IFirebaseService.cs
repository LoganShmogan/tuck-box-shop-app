using Firebase.Database;

namespace tuck_box_shop.Services
{
    public interface IFirebaseService
    {
        Task<T> GetDataAsync<T>(string path);
        Task SaveDataAsync<T>(string path, T data);
        Task UpdateDataAsync<T>(string path, T data);
        Task DeleteDataAsync(string path);
        IObservable<FirebaseEvent<T>> ObserveData<T>(string path);
    }
}