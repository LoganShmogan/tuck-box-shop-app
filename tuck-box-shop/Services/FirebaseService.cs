using Firebase.Database;
using Newtonsoft.Json;

namespace tuck_box_shop.Services
{
    public class FirebaseService : IFirebaseService
    {
        private FirebaseClient _firebase;
        
        public FirebaseService()
        {
            // Replace with your actual Firebase Database URL
            _firebase = new FirebaseClient("YOUR_DATABASE_URL_HERE");
        }
        
        public async Task<T> GetDataAsync<T>(string path)
        {
            try
            {
                var result = await _firebase.Child(path).OnceSingleAsync<T>();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get data error: {ex.Message}");
                return default(T);
            }
        }
        
        public async Task SaveDataAsync<T>(string path, T data)
        {
            try
            {
                await _firebase.Child(path).PutAsync(JsonConvert.SerializeObject(data));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Save data error: {ex.Message}");
            }
        }
        
        public async Task UpdateDataAsync<T>(string path, T data)
        {
            try
            {
                await _firebase.Child(path).PatchAsync(JsonConvert.SerializeObject(data));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update data error: {ex.Message}");
            }
        }
        
        public async Task DeleteDataAsync(string path)
        {
            try
            {
                await _firebase.Child(path).DeleteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete data error: {ex.Message}");
            }
        }
        
        public IObservable<FirebaseEvent<T>> ObserveData<T>(string path)
        {
            return _firebase.Child(path).AsObservable<T>();
        }
    }
}