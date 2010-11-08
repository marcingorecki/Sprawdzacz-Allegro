using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Threading;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace Lista_Przedmiotow
{
    class Controller
    {
        private DataContainer data = new DataContainer();
        private AllegroWebApiService allegroWebApiService = new AllegroWebApiService();
        public ListBox logBox;

        private static int MYACCOUNTSTRUCT_ID_INDEX = 0;
        private static int MYACCOUNTSTRUCT_TITLE_INDEX = 9;
        
        /// <summary>
        /// Main routine of computation engine. Retrieves data from Allegro, then runs calculations on them.
        /// </summary>
        public void calculate()
        {
            data.clear();
            
            try 
            {
                log("Pobieram sprzedane");
                foreach (String number in gcNumbersOnly(getRawData(DataTypes.SOLD)))
                {
                    data.sold.Add(number);                
                }
                data.sold.Sort();            

                log("Pobieram niesprzedane");
                foreach (String number in gcNumbersOnly(getRawData(DataTypes.UNSOLD)))
                {
                    data.unsold.Add(number);
                }
                data.unsold.Sort();

                log("Pobieram sprzedawane");
                foreach (String number in gcNumbersOnly(getRawData(DataTypes.SELLING)))
                {
                    data.selling.Add(number);
                }
                data.selling.Sort();

                log("Licze brakujace");
                calculateMissing();

                log("Licze duplikaty");
                calculateDuplicates();
            }
            catch (SoapException e)
            {
                log("Problem z połączeniem, sprawdz dane. ["+e.Message+"]");
            }

        }

        /// <summary>
        /// Returns DataContainer object being used by controller
        /// </summary>
        /// <returns></returns>
        public DataContainer getDataContainer(){
            return data;
        }

        /// <summary>
        /// Logs data to log box in main window (or in fact in any listbox that has been set in controller during setup)
        /// </summary>
        /// <param name="value"></param>
        private void log(String value)
        {
            logBox.Dispatcher.Invoke(
                System.Windows.Threading.DispatcherPriority.Normal,
                    new Action(
                        delegate()
                        {
                            logBox.Items.Insert(0, "[" + DateTime.Now.ToString("HH:mm:ss") + "]" + value);
                        }
                ));
        }

        /// <summary>
        /// Calculates duplicate items
        /// </summary>
        private void calculateDuplicates()
        {
            // count all items using hashtable
            Hashtable map = new Hashtable();
            foreach (String item in data.selling)
            {
                if (map.ContainsKey(item))
                {
                    map[item] = (int)map[item] + 1;
                }
                else
                {
                    map.Add(item, 1);
                }
            }
            // add all duplicates (items having count >1) to data.duplicates list
            foreach (String duplicate in map.Keys)
            {
                if ((int)map[duplicate] > 1)
                {
                    data.duplicates.Add(duplicate);
                }
            }
            data.duplicates.Sort();
        }

        /// <summary>
        /// Calculate Missing items
        /// </summary>
        private void calculateMissing()
        {
            calculateMissingFrom(data.sold);
            calculateMissingFrom(data.unsold);
        }

        /// <summary>
        /// subroutine for calculate missing items - perform operations on given list
        /// </summary>
        /// <param name="dataList"></param>
        private void calculateMissingFrom(List<String> dataList)
        {
            //iterat through given list and find all data that are not in data.selling list
            foreach (String item in dataList)
            {
                if (!data.selling.Contains(item))
                {
                    data.missing.Add(item);
                }
            }
            //remove duplicates
            data.missing = data.missing.Distinct().ToList();
        }

        /// <summary>
        /// Return list of numbers calculated from auction titles 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private List<String> gcNumbersOnly(List<String> input)
        {
            List<String> numbers=new List<String>();
            Regex gcNumberPattern = new Regex("[A-Z]\\d{3,4}[a-zA-Z]?");
            foreach (String item in input)
            {
                Match match=gcNumberPattern.Match(item);
                if (match.Success)
                {
                    numbers.Add(match.ToString());
                }
            }
            return numbers;
        }

        /// <summary>
        /// Connect to SOAP service and retrieve list of auctions
        /// </summary>
        /// <param name="type">Auction collection to retrieve</param>
        /// <returns></returns>
        private List<String> getRawData(String type) 
        {
            List<String> rawData = new List<String>();
            long userid;
            long servertime;

            //connect to Allegro
            String sessionHandle=sessionHandle = allegroWebApiService.doLogin(data.userLogin, data.userPassword, data.countryCode, data.webapiKey, data.localVersion, out userid, out servertime);

            //data are retrieved in chunks of 100. Loop and retrieve all data
            bool endOfData = false;
            int start = 0;
            while (!endOfData)
            {
                //get structure containing all the data
                MyAccountStruct2[] results = allegroWebApiService.doMyAccount2(sessionHandle, type, start, new int[] { }, 100);
                start += 100;

                //Are there any data to retrieve left?
                if (results.Length == 0)
                {
                    endOfData = true;
                }

                //add all returend items' id and title to list
                foreach (MyAccountStruct2 result in results)
                {
                    rawData.Add(result.myaccountarray[MYACCOUNTSTRUCT_ID_INDEX] + "-" + result.myaccountarray[MYACCOUNTSTRUCT_TITLE_INDEX]);
                }
            }

 	        return rawData;
        }
    }
}
