using DGRA_V1.Repository.Interface;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Net;
using System.Text;

namespace DGRA_V1.Models
{
    public class WindUploadingFileValidation
    {
        ErrorLog m_ErrorLog;
        Hashtable m_DeviceCollection = new Hashtable();
        IDapperRepository _idapperRepo;

        // Adding key/value pair
        // in the hashtable
        // Using Add() method
        //Console.WriteLine("Key and Value pairs from my_hashtable1:");
        //foreach(DictionaryEntry ele1 in my_hashtable1)
        //{
        //    Console.WriteLine("{0} and {1} ", ele1.Key, ele1.Value);
        //}

        public WindUploadingFileValidation()
        { }
        ~WindUploadingFileValidation()
        { }

        public WindUploadingFileValidation(ErrorLog arErrorLog, IDapperRepository iRepo)
        {
            m_ErrorLog = arErrorLog;
            _idapperRepo = iRepo;
            DataTable dTable = new DataTable();
            var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindLocationMaster";
            var result = string.Empty;

            WebRequest request = WebRequest.Create(url);
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                Stream receiveStream = response.GetResponseStream();
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    result = readStream.ReadToEnd();
                }
                dTable = JsonConvert.DeserializeObject<DataTable>(result);
            }

            foreach (DataRow dr in dTable.Rows)
            {
                m_DeviceCollection.Add((string)dr["wtg"], (string)dr["site"]);
            }
        }
        public string breakDownCalc(string stopFrom, string stopTo)
        {

            DateTime stopFrom_ = Convert.ToDateTime(stopFrom);
            DateTime stopTo_ = Convert.ToDateTime(stopTo);
            TimeSpan totalStop_ = stopTo_ - stopFrom_;
            string totalStop = Convert.ToString(totalStop_);
            return totalStop;
        }
        public bool validateBreakDownData(long rowNumber, string breakDownType, string verifyWTG, string stopFrom, string stopTo)
        {
            bool greaterStopTo = false;
            bool lastStopTo = false;
            bool totalStop = false;
            bool wtgFlag = false;

            //1)(Stop To) – column always greater than (Stop From) – column.
            DateTime stopTo_ = Convert.ToDateTime(stopTo);
            DateTime stopFrom_ = Convert.ToDateTime(stopFrom);
            if (stopFrom_> stopTo_)
            {
                greaterStopTo = true;
            }

            if (!(m_DeviceCollection.ContainsKey(verifyWTG)))
            {
                wtgFlag = true;
            }

            //2)(Stop To) Hours - column 24:00:00 should be 23:59:59
            if (stopTo =="24:00:00")
            {
                lastStopTo = true;
            }

            //3)BD Hours should not be more than 24 Hrs.
            TimeSpan bdHours = (stopTo_ - stopFrom_);
            if (bdHours.Hours>24)
            {
                totalStop = true;
            }

            //4)Production_hrs+Lull Hrs+USMH+SMH+Others Hour+IGBD+EGBD+LoadShedding = 24 Hrs;
            //Production_hrs,Lull Hrs, USMH, SMH, Others Hour,IGBD,EGBD,LoadShedding should not be negative.
            //if ()
            //{
            //}

            //*Overall Error Status*
            if (wtgFlag == true || greaterStopTo == true || lastStopTo == true || totalStop == true )
            {
                //|| sumOfBDHours == true

                if (wtgFlag == true)
                {
                    m_ErrorLog.SetError(",File Row [" + rowNumber + "] Invalid - Equipment record does not exist : ");
                }
                if (greaterStopTo == true)
                {
                    m_ErrorLog.SetError(",File Row [" + rowNumber + "] Stop-To Time is lower than Stop-From: ");
                }
                if (lastStopTo == true)
                {
                    m_ErrorLog.SetError(",File Row [" + rowNumber + "] Invalid  Stop-To timing:");
                }
                if (totalStop == true)
                {
                    m_ErrorLog.SetError(",File Row [" + rowNumber + "] Breakdown Hours are exceeding 24 hours:");
                }
                //if (sumOfBDHours == true)
                //{
                //    m_ErrorLog.SetError(",Sum of all breakdown hours exceed 24 hours: ");
                //}
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool validateGenData(long rowNumber, string verifyDate, string verifyWTG, double verifyWSpeed, double verifyKWh, double operatingHrs, double productionHrs, double gridHrs)
        {
            //Here flags are error flags which are assigned true if the validation condition is not satisfied
            bool dateFlag = false;
            bool wtgFlag = false;
            bool wSpeedFlag = false;
            bool kwhFlag = false;
            bool gridHrsFlag = false;

            //1)Date Validation CodeBlock
            string checker = (Convert.ToDateTime(verifyDate)).ToString("yyyy-MM-dd");
            if (!(verifyDate == checker))
            {
                dateFlag = true;
                m_ErrorLog.SetError(",File Row <" + rowNumber + "> Invalid Date Format : " + verifyDate);
            }

            //2)WTG Validation CodeBlock
            if (!(m_DeviceCollection.ContainsKey(verifyWTG)))
            {
                wtgFlag = true;
                m_ErrorLog.SetError(",File Row <" + rowNumber + "> Equipment name("+ verifyWTG + ") record does not exist " );
            }

            //3)windSpeed Validation CodeBlock
            if (!(verifyWSpeed > 0))
            {
                wSpeedFlag = true;
                m_ErrorLog.SetError(",File Row <" + rowNumber + "> Wind speed cannot be zero or negative: " + verifyWSpeed);
            }

            //4)KWh Validation CodeBlock
            string emptyVal = Convert.ToString(verifyKWh);
            if (verifyKWh < 0)
            {
                kwhFlag = true;
                m_ErrorLog.SetError(",File Row <" + rowNumber + "> KWH cannot be negative: " + verifyKWh);
            }
            else if (string.IsNullOrEmpty(emptyVal))
            {
                kwhFlag = true;
                m_ErrorLog.SetError(",File Row <" + rowNumber + "> KWH cannot be blank: " + verifyKWh);
            }
            else if (verifyKWh == 0)
            {
                if (!(operatingHrs == 0) || !(productionHrs == 0))
                {
                    kwhFlag = true;
                    m_ErrorLog.SetError(",File Row <" + rowNumber + "> If KWH is zero then Generation/Production hours should be zero: " + verifyKWh);
                }
            }

            //5)Grid Hours Validation CodeBlock
            if (!(gridHrs >= productionHrs))
            {
                gridHrsFlag = true;
                m_ErrorLog.SetError(",File Row <" + rowNumber + "> Grid Hours should be greater than or equal to production hours: " + gridHrs);

            }

            //*Overall Error Status*
            if (dateFlag == true || wtgFlag == true || wSpeedFlag == true || kwhFlag == true || gridHrsFlag == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}


