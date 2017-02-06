namespace WowSuite.Launcher.Config
{
    public class AuthData
    {        
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:AuthData"/>.
        /// </summary>
        public AuthData() 
            : this(string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="T:AuthData"/>.
        /// </summary>
        public AuthData(string login, string password)
        {
            Login = login;
            Password = password;
        }

        /// <summary>
        /// Логин
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; set; }
    }
}
