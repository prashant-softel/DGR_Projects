﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Collections;
using System.IO;
using System.Text;

//using System.Linq;
//using System.Threading.Tasks;

namespace DGRA_V1.Models
{
    public class ErrorLog
    {
        private IWebHostEnvironment env;
        public ErrorLog(IWebHostEnvironment obj)
        {
            env = obj;
        }
        ~ErrorLog()
        {
        }
        //Collection of message
        public ArrayList messageArray = new ArrayList();
        int errorCount;

        class cMessage
        {
            //Int TypeOfMsg, //1 = nifo, 2= warning, 3 = Error
            public enum messageType
            {
                Information,
                Warning,
                Error
            }
            messageType m_messageType;
            string m_sMessage;
            public messageType Get_MessageType()
            {
                return m_messageType;
            }

            public string Get_Message()
            {
                return m_sMessage;
            }


            public string Get_FormatedMessage(string sMessage)
            {
                switch (m_messageType)
                {
                    case messageType.Information:
                        sMessage = "Information : " + sMessage;
                        break;
                    case messageType.Warning:
                        sMessage = "Warning : " + sMessage;
                        break;
                    case messageType.Error:
                        sMessage = "Error : " + sMessage;
                        break;
                }
                return sMessage;
            }


            public cMessage(cMessage.messageType mtype, string sMessage)
            {
                m_messageType = mtype;
                m_sMessage = sMessage;
            }

        }

        public void Clear()
        {
            errorCount = 0;
            messageArray.Clear();
        }

        public void SetError(string sMsg)
        {
            //Pushed or inserted at end of the collection
            //Create a message
            //Add the message to the collection
            if (!(string.IsNullOrEmpty(sMsg)))
            {
                cMessage objMessage = new cMessage(cMessage.messageType.Error, sMsg);
                messageArray.Add(objMessage);
                errorCount++;
            }
        }


        public void SetInformation(string sMsg)
        {
            //Pushed or inserted at end of the collection
            //m_Messages.Add(new CMessage(1, sMsg));
            if (!(string.IsNullOrEmpty(sMsg)))
            {
                cMessage objMessage = new cMessage(cMessage.messageType.Information, sMsg);
                messageArray.Add(objMessage);
            }
        }

        public void SetWarning(string sMsg)
        {
            //Pushed or inserted at end of the collection
            //m_Messages.Add(new CMessage(2, sMsg));
            if (!(string.IsNullOrEmpty(sMsg)))
            {
                cMessage objMessage = new cMessage(cMessage.messageType.Warning, sMsg);
                messageArray.Add(objMessage);
            }
        }

        public int GetErrorCount()
        {
            return errorCount;
        }

        void ShowResults()
        {

        }

        public void SaveToCSV(string csvPath)
        {
            string sMessage = "";
            StringBuilder content = new StringBuilder();
            foreach (cMessage msg in messageArray)
            {
                string indexMsg = msg.Get_Message();
                sMessage = msg.Get_FormatedMessage(indexMsg);
                content.AppendLine(sMessage);
            }
            //csvPath = env.ContentRootPath + @"\FileLog\" + csvPath;
             csvPath = @"C:\LogFile\" + csvPath; 
            File.AppendAllText(csvPath, Convert.ToString(content));
            sMessage = "Total errors <" + errorCount + ">";


        }

        public ArrayList errorLog()
        {
            string sMessage = "";
            ArrayList messageList = new ArrayList();
            foreach (cMessage msg in messageArray)
            {
                string indexMsg = msg.Get_Message();
                sMessage = msg.Get_FormatedMessage(indexMsg);
                messageList.Add(sMessage);
            }
            return messageList;
        }
    }

}
