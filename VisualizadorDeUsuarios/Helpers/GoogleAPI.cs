using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TesiNexus.Helpers
{
    public class GoogleAPI
    {
        public static async Task DownloadFileAsync(string originFolder,string fileName, bool? saveFile = false)
        {
            try
            {         
                //lembrar de pegar json da estrutura do projeto
                var path = @"D:\Felipe\Arquivos\Json\credential.json";

                var tokenStorage = new FileDataStore("UserCredentialStoragePath", true);

                UserCredential credential;

                await using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        new[] { DriveService.ScopeConstants.DriveReadonly },
                        "userName",
                        CancellationToken.None,
                        tokenStorage).Result;
                }

                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential
                });

                var request = service.Files.List();
                request.Q = $"mimeType = 'application/vnd.google-apps.folder' and name contains '{originFolder}'";

                var result = await request.ExecuteAsync();

                var filesInParent = service.Files.List();
                filesInParent.Q = $"parents in '{result.Files.FirstOrDefault().Id}'";
                
                var allFilesInParentResponse = filesInParent.Execute();

                foreach (var file in allFilesInParentResponse.Files)
                {
                    if (file.Name.Equals(fileName))
                    {
                        //fazer download
                        var r = service.Files.Get(file.Id);
                        var stream = new MemoryStream();

                        if (saveFile == true)
                        {
                            //chamar dialog e escolher local
                        }
                        else
                        {
                            var saveFolder = Directory.GetCurrentDirectory() + $"\\{fileName}";
                            r.Download(stream);

                            //Por algum motivo esta corrompendo o arquivo, verificar
                            using (FileStream f = new FileStream(saveFolder, FileMode.Create, System.IO.FileAccess.Write))
                            {
                                byte[] bytes = new byte[stream.Length];
                                stream.Read(bytes, 0, (int)stream.Length);
                                f.Write(bytes, 0, bytes.Length);
                                stream.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static async Task DownloadDataBaseAsync()
        {
            try
            {
                //lembrar de pegar json da estrutura do projeto
                // var path = @"D:\Felipe\Arquivos\Json\credential.json";
                 var path = @"D:\A ERA DE ULTRON\credential.json";

                var tokenStorage = new FileDataStore("UserCredentialStoragePath", true);

                UserCredential credential;

                await using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        new[] { DriveService.ScopeConstants.DriveReadonly },
                        "userName",
                        CancellationToken.None,
                        tokenStorage).Result;
                }

                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential
                });

                var request = service.Files.List();
                request.Q = $"name contains 'VVAND4.bak'";

                var result = await request.ExecuteAsync();

                var file = result.Files.FirstOrDefault();
                var getRequest = service.Files.Get(file.Id);

                string programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                string folder = programData + "\\NexusConfig\\Temp";
                string downloadFile = folder + $"\\{file.Name}";

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                else
                {
                    if(File.Exists(downloadFile))
                      File.Delete(downloadFile);
                }

                await using var fileStream = new FileStream(downloadFile, FileMode.Create, FileAccess.Write);
                await getRequest.DownloadAsync(fileStream);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static async Task<string> CheckLatestVersionAsync(string currentVersion)
        {
            try
            {
                //lembrar de pegar json da estrutura do projeto
                // var path = @"D:\Felipe\Arquivos\Json\credential.json";
                var path = @"D:\A ERA DE ULTRON\credential.json";

                var tokenStorage = new FileDataStore("UserCredentialStoragePath", true);

                UserCredential credential;

                await using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        new[] { DriveService.ScopeConstants.DriveReadonly },
                        "userName",
                        CancellationToken.None,
                        tokenStorage).Result;
                }

                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential
                });

                var latestVersion = service.Files.List();
                latestVersion.Q = $"parents in '1y8XxdR6dAITXoJV2thGKTsXfsyxiYhYJ'";

                var result = await latestVersion.ExecuteAsync();

                var file = result.Files.FirstOrDefault();
                currentVersion += ".sql";
                if (!currentVersion.Equals(file.Name))
                {
                    var getRequest = service.Files.Get(file.Id);

                    string programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                    string folder = programData + "\\NexusConfig\\Temp";
                    string downloadFile = folder + $"\\{file.Name}";

                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                    else
                    {
                        if (File.Exists(downloadFile))
                            File.Delete(downloadFile);
                    }

                    await using var fileStream = new FileStream(downloadFile, FileMode.Create, FileAccess.Write);
                    await getRequest.DownloadAsync(fileStream);

                    return downloadFile;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

    }
}
