/*
Copyright 2018 Google Inc

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    https://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;

namespace FTM.WebApi
{
    /// <summary>
    /// Client auth information, loaded from a Google user credential json file.
    /// Set the TEST_CLIENT_SECRET_FILENAME environment variable to point to the credential file.
    /// </summary>
    public class ClientInfo
    {
        public static ClientInfo Load()
        {
            const string ClientSecretFilenameVariable = "client_id.json";
            string clientSecretFilename = Environment.GetEnvironmentVariable(ClientSecretFilenameVariable);
            //if (string.IsNullOrEmpty(clientSecretFilename))
            //{
            //    throw new InvalidOperationException($"Please set the {ClientSecretFilenameVariable} environment variable before running tests.");
            //}
            var secrets = JObject.Parse(Encoding.UTF8.GetString(File.ReadAllBytes("C:\\client_id.json")))["web"];
            var projectId = secrets["project_id"].Value<string>();
            var clientId = secrets["client_id"].Value<string>();
            var clientSecret = secrets["client_secret"].Value<string>();
            return new ClientInfo(projectId, clientId, clientSecret);
        }

        private ClientInfo(string projectId, string clientId, string clientSecret)
        {
            ProjectId = projectId;
            ClientId = clientId;
            ClientSecret = clientSecret;
            //AuthUri = "https://accounts.google.com/o/oauth2/auth";
            //"token_uri": "https://oauth2.googleapis.com/token",
            //"auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
        }

        public string ProjectId { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }
    }
}
