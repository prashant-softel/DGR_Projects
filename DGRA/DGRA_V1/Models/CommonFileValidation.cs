using DGRA_V1.Repository.Interface;
using System;

namespace DGRA_V1.Models
{
    public class CommonFileValidation
    {
        ErrorLog m_ErrorLog;
        IDapperRepository _idapperRepo;
        //List<string> eqList = new List<string> { };

        // Adding key/value pair
        // in the hashtable
        // Using Add() method
        //Console.WriteLine("Key and Value pairs from my_hashtable1:");
        //foreach(DictionaryEntry ele1 in my_hashtable1)
        //{
        //    Console.WriteLine("{0} and {1} ", ele1.Key, ele1.Value);
        //}

        public CommonFileValidation()
        { }
        ~CommonFileValidation()
        { }

        public CommonFileValidation(ErrorLog arErrorLog, IDapperRepository iRepo)
        {
            m_ErrorLog = arErrorLog;
            _idapperRepo = iRepo;
            //DataTable dTable = new DataTable();
            //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarLocationMaster";
            //var result = string.Empty;
            //WebRequest request = WebRequest.Create(url);
            //using (var response = (HttpWebResponse)request.GetResponse())
            //{
            //    Stream receiveStream = response.GetResponseStream();
            //    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
            //    {
            //        result = readStream.ReadToEnd();
            //    }
            //    dTable = JsonConvert.DeserializeObject<DataTable>(result);
            //}

            //foreach (DataRow dr in dTable.Rows)
            //{
            //    eqList.Add((string)dr["icr_inv"]);
            //}
        }

        public double stringToPercentage(long rowNo, string excelValue, string columnName)
        {
            double percentage = 0;
            if (!(string.IsNullOrEmpty(excelValue)))
            {
                excelValue = excelValue.Replace("%", "");
                excelValue = excelValue.Replace(" ", "");
                percentage = Convert.ToDouble(excelValue);
                if (percentage > 100)
                {
                    m_ErrorLog.SetError(",File Row <" + rowNo + "> Percentage value in column <" + columnName + "> exceeds 100% value");
                }
                /*else if (percentage < 0)
                {
                    m_ErrorLog.SetError(",File Row <" + rowNo + "> Percentage value in column <" + columnName + "> is negative");
                }*/
            }
            else
            {
                return 0;
            }

            return percentage;
        }


    }
}


