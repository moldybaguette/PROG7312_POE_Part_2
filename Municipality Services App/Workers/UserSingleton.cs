using Municipality_Services_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Municipality_Services_App.Workers
{
    public class UserSingleton
    {
        // The single instance of UserSingleton (lazy initialized)
        private static UserSingleton _instance;

        // Lock object for thread-safe singleton instantiation
        private static readonly object _lock = new object();

        // The User object to be stored
        public User CurrentUser { get; private set; }

        // Private constructor to prevent instantiation from other classes
        private UserSingleton() { }

        /// <summary>
        /// Gets the instance of the UserSingleton.
        /// This method ensures that only one instance of UserSingleton is created.
        /// </summary>
        public static UserSingleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new UserSingleton();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Sets the current User object.
        /// This method allows setting the User instance that the singleton will manage.
        /// </summary>
        /// <param name="user">The User object to set as the current CurrentUser.</param>
        public void SetUser(User user)
        {
            this.CurrentUser = user;
        }
    }
}
//----------------------------------------------------End_of_File----------------------------------------------------//