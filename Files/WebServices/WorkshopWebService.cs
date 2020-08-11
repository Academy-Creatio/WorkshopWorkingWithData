using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using Terrasoft.Core;
using Terrasoft.Web.Common;
using WorkshopWorkingWithData.Files.DataOperations;

namespace WorkshopWorkingWithData
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class WorkshopWebService : BaseService
    {
        private ReadingData ReadingData { get; set; }
        private Stopwatch Timer { get; set;}
        public WorkshopWebService()
        {
            ReadingData = new ReadingData(UserConnection ?? SystemUserConnection);
            Timer = new Stopwatch();
        }

        #region Properties
        private SystemUserConnection _systemUserConnection;
        private SystemUserConnection SystemUserConnection
        {
            get
            {
                return _systemUserConnection ??= (SystemUserConnection)AppConnection.SystemUserConnection;
            }
        }
        #endregion

        #region Methods : REST
        [OperationContract]
        [WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Xml)]
        public Stream GetAllContacts(QuryType qt)
        {
            //http://localhost:6090/0/rest/WorkshopWebService/GetAllContacts?qt=0
            Timer.Start();
            Tuple<DataTable, string> result;
            switch (qt)
            {
                case QuryType.SELECT:
                    result = ReadingData.GetAllContactsSelect();
                    break;
                case QuryType.ESQ:
                    result = ReadingData.GetAllContactsEsq();
                    break;
                case QuryType.CustomQuery:
                    result = ReadingData.GetAllContactsCustomQuery();
                    break;
                default:
                    result = ReadingData.GetAllContactsSelect();
                    break;
            }
            Timer.Stop();

            string htmlPage = result.Item1.GetHtmlPage(Timer.ElapsedTicks, result.Item2, qt.ToString());
            return GetMemoryStream(htmlPage);
        }

        [OperationContract]
        [WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Xml)]
        public Stream GetFilteredContacts(QuryType qt, string email)
        {
            //http://localhost:6090/0/rest/WorkshopWebService/GetFilteredContacts?qt=0&email=a.baker@ac.com	
            Timer.Start();
            Tuple<DataTable, string> result;
            switch (qt)
            {
                case QuryType.SELECT:
                   result = ReadingData.GetFilteredContactsSelect(email);
                    break;
                case QuryType.ESQ:
                    result = ReadingData.GetFilteredContactsEsq(email);
                    break;
                case QuryType.CustomQuery:
                    result = ReadingData.GetFilteredContactsCustomQuery(email);
                    break;
                default:
                    result = ReadingData.GetFilteredContactsSelect(email);
                    break;
            }
            Timer.Stop();

            string htmlPage = result.Item1.GetHtmlPage(Timer.ElapsedTicks, result.Item2, qt.ToString());
            return GetMemoryStream(htmlPage);
        }
        #endregion

        #region Methods : Private
        private MemoryStream GetMemoryStream(string htmlPage)
        {
            byte[] data = Encoding.UTF8.GetBytes(htmlPage);
            string contentType = "text/html; charset=utf-8";
            WebOperationContext.Current.OutgoingResponse.ContentType = contentType;
            WebOperationContext.Current.OutgoingResponse.ContentLength = data.Length;
            return new MemoryStream(data);
        }
        #endregion
    }
}
