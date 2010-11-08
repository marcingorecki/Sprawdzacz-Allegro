using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Lista_Przedmiotow
{
    /// <summary>
    /// Class containing a set of data (connection data & values list)
    /// </summary>
    public class DataContainer
    {
        // Auction data
        public List<String> sold = new List<String>();
        public List<String> unsold = new List<String>();
        public List<String> selling = new List<String>();
        public List<String> missing = new List<String>();
        public List<String> duplicates = new List<String>();

        //connection data
        public String userLogin = "";
        public String userPassword = "";
        public int countryCode = 0;
        public String webapiKey = "";
        public int localVersion = 0;

        //constants
        private static String regKey = "Software\\net.mgorecki.Sprawdzacz";
        private static String USERLOGINKEY = "userLogin";
        private static String USERPASSWORDKEY = "userPassword";
        private static String COUNTRYCODEKEY = "countryCode";
        private static String WEBAPIKEYKEY = "webApiKey";
        private static String LOCALVERSIONKEY = "localVersion";

        public DataContainer()
        {
            load();
        }

        /// <summary>
        /// Clear all auction data.
        /// Connection data remain unmodified.
        /// </summary>
        internal void clear()
        {
            sold.Clear();
            unsold.Clear();
            selling.Clear();
            missing.Clear();
            duplicates.Clear();
        }

        /// <summary>
        /// Load connection data form registry
        /// </summary>
        public void load()
        {
            RegistryKey softwareKey = Registry.CurrentUser.OpenSubKey(regKey);
            if (softwareKey != null)
            {
                userLogin = NullSafeToString(softwareKey.GetValue(USERLOGINKEY));
                userPassword = NullSafeToString(softwareKey.GetValue(USERPASSWORDKEY));
                int.TryParse(NullSafeToString(softwareKey.GetValue(COUNTRYCODEKEY)), out countryCode );
                webapiKey = NullSafeToString(softwareKey.GetValue(WEBAPIKEYKEY));
                int.TryParse(NullSafeToString(softwareKey.GetValue(LOCALVERSIONKEY)), out localVersion);
            }
        }

        /// <summary>
        /// Save connection data to registry
        /// </summary>
        public void save()
        {
            RegistryKey softwareKey = Registry.CurrentUser.OpenSubKey(regKey, true);
            if (softwareKey == null)
            {
                softwareKey = Registry.CurrentUser.CreateSubKey(regKey);
            }
            softwareKey.SetValue(USERLOGINKEY, userLogin);
            softwareKey.SetValue(USERPASSWORDKEY, userPassword);
            softwareKey.SetValue(COUNTRYCODEKEY, countryCode.ToString() );
            softwareKey.SetValue(WEBAPIKEYKEY, webapiKey);
            softwareKey.SetValue(LOCALVERSIONKEY, localVersion.ToString());

        }

        /// <summary>
        /// Allow ToString() operation on null objects.
        /// </summary>
        /// <param name="o">Object to be casted ToString</param>
        /// <returns>ToString() if input is not null, empty string otherwise</returns>
        private String NullSafeToString(Object o)
        {
            if (o != null)
            {
                return o.ToString();
            }
            else
            {
                return "";
            }
        }
    }
}
