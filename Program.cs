using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3
{



    /// <summary>
    /// Data Formats in Cloud Search
    /// </summary>
    public enum DataFormat
    {
        Json, Xml
    }

    public class CloudSearch
    {

        /// <summary>
        /// Used to Add/Update/Delete Document batch data to Amazon Cloud Search
        /// </summary>
        /// <param name="JsonFormat"></param>
        /// <param name="DocumentUri"></param>
        /// <param name="ApiVersion"></param>
        /// <returns>Response Status</returns>    
        public string PostDocuments(string JsonFormat, string DocumentUri, string ApiVersion, DataFormat format)
        {
            try
            {
                var request = (HttpWebRequest)
                WebRequest.Create(string.Format("http://{0}{1}batch", DocumentUri, ApiVersion));
                request.ProtocolVersion = HttpVersion.Version11;
                request.Method = "POST";
                byte[] postBytes = Encoding.UTF8.GetBytes(JsonFormat);
                request.ContentLength = postBytes.Length;
                request.Accept = "application/json";
                request.ContentType = format == DataFormat.Xml ? "text/xml;charset=utf-8" : "application/json;charset=utf-8";
                var requestStream = request.GetRequestStream();
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();
                HttpWebResponse response = null;
                response = (HttpWebResponse)request.GetResponse();
                var retVal = new StreamReader(stream: response.GetResponseStream()).ReadToEnd();
                var statusCode = response.StatusCode;
                return statusCode + retVal;
            }
            catch (WebException Ex)
            {
                throw Ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Used to make search Request to Amazon Cloud Search
        /// </summary>
        /// <param name="SearchUri"></param>
        /// <param name="SearchQuery"></param>
        /// <param name="ReturnFields"></param>
        /// <param name="PageSize"></param>
        /// <param name="Start"></param>
        /// <returns>Response Status</returns>      
        public string SearchRquest(string SearchUri, string SearchQuery, string ReturnFields, int PageSize, int Start, DataFormat format)
        {
            try
            {
                string SearchUrl = SearchUri + SearchQuery + "&return-fields=" + ReturnFields + "&start=" + Start + "&size=" + PageSize;
                if (format == DataFormat.Xml)
                    SearchUrl = SearchUrl + "&results-type=xml";

                string responseFromServer = string.Empty;
                // Create a request for the URL.
                WebRequest request = WebRequest.Create(SearchUrl);
                // If required by the server, set the credentials.
                request.Credentials = CredentialCache.DefaultCredentials;
                request.ContentType = "application/json";
                // Get the response.
                WebResponse response = request.GetResponse();
                // Get the stream containing content returned by the server.
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                responseFromServer = reader.ReadToEnd();
                // Clean up the streams and the response.
                reader.Close();
                response.Close();
                //returns response from the server
                return responseFromServer;
            }
            catch (WebException Ex)
            {
                throw Ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string SampleSearch(string domainName, string searchQuery, DataFormat format, string returnFields, string pageSize, string startIndex)
        {
            try
            {
                string strTemplateUrl = "{SearchEndPoint}/{ApiVersion}/search?q={SearchKeyWord}" +
                    "&format={DataFormat}&return={ReturnFields}&size={PageSize}&start={StartIndex}";
                string strSearchEndPoint = "http://search-{DomainName}.us-east-1.cloudsearch.amazonaws.com";
                strSearchEndPoint = strSearchEndPoint.Replace("{DomainName}", domainName);
                strTemplateUrl = strTemplateUrl.Replace("{SearchEndPoint}", strSearchEndPoint);
                strTemplateUrl = strTemplateUrl.Replace("{ApiVersion}", "2013-01-01");
                strTemplateUrl = strTemplateUrl.Replace("{SearchKeyWord}", searchQuery);
                strTemplateUrl = strTemplateUrl.Replace("{DataFormat}", (format == DataFormat.Xml ? "xml" : "json"));
                strTemplateUrl = strTemplateUrl.Replace("{ReturnFields}", returnFields);
                strTemplateUrl = strTemplateUrl.Replace("{PageSize}", pageSize);
                strTemplateUrl = strTemplateUrl.Replace("{StartIndex}", startIndex);
                return MakeWebRequest(strTemplateUrl);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string MakeWebRequest(string searchUrl)
        {
            try
            {
                string responseFromServer = string.Empty;
                WebRequest request = WebRequest.Create(searchUrl);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.ContentType = "application/json";
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();
                reader.Close();
                response.Close();
                return responseFromServer;

            }
            catch (Exception)
            {
                throw;
            }
        }   
    }

    

    class Program
    {
        static void Main(string[] args)
        {

            CloudSearch ObjCloudSearch = new CloudSearch();
            string strResponse = string.Empty;
            //if (IsSortByViews)
            //    strQuery = strQuery + "&rank=-vw";
            //else if (isSortByLatest)
            //    strQuery = strQuery + "&rank=-pid";
            //Call Amazon Cloud Search API Service -- views and contentid are the indexed fields
            //strResponse = ObjCloudSearch.SearchRquest(configSection.SearchUri, strQuery, strReturnFields, p_intPageSize, p_intStart, DataFormat.Json);
            strResponse = ObjCloudSearch.SampleSearch("seranatamvc-2-twxbozw3iawub7pvx2uxtksmlm", "roses", DataFormat.Xml, "_all_fields", "8", "0");
            Console.WriteLine(strResponse);
            

        }
    }
}
