using Firebase.Auth;
using Firebase.Database;
using Newtonsoft.Json;
using tuck_box_shop.Models;

namespace tuck_box_shop.Services
{
    public class AuthService : IAuthService
    {
        private FirebaseAuthProvider _authProvider;
        private FirebaseAuthLink _authLink;
        private IFirebaseService _firebaseService;
        
        public AuthService(IFirebaseService firebaseService)
        {
            // Replace with your actual Firebase Web API Key
            _authProvider = new FirebaseAuthProvider(new FirebaseConfig("YOUR_WEB_API_KEY_HERE"));
            _firebaseService = firebaseService;
        }
        
        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                _authLink = await _authProvider.SignInWithEmailAndPasswordAsync(email, password);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return false;
            }
        }
        
        public async Task<bool> RegisterAsync(string email, string password, User userData)
        {
            try
            {
                // Create user in Firebase Authentication
                _authLink = await _authProvider.CreateUserWithEmailAndPasswordAsync(email, password);
                
                // Save additional user data to Realtime Database
                userData.FirebaseKey = _authLink.User.LocalId;
                await _firebaseService.SaveDataAsync($"users/{_authLink.User.LocalId}", userData);
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration error: {ex.Message}");
                return false;
            }
        }
        
        public string GetCurrentUserId()
        {
            return _authLink?.User?.LocalId;
        }
        
        public bool IsUserLoggedIn()
        {
            return _authLink != null && !string.IsNullOrEmpty(_authLink.User?.LocalId);
        }
        
        public async Task<bool> LogoutAsync()
        {
            try
            {
                _authLink = null;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}