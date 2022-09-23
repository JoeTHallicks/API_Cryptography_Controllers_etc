using System;
using System.Text;
using DistSysAcwClient.Class;

namespace DistSysAcwClient
{
    #region 
    internal class Client
    {
        private static void Main()
        {
            Console.WriteLine("Hello. What would you like to do?");
            var userInput = Console.ReadLine();
            while (userInput?.ToLower() != "exit")
            {
                var response = string.Empty;
                try
                {
                    var splitInput = userInput?.Split(' ');
                    switch (splitInput)
                    {
                        case { } x when x[0].ToLower() == "Talkback" && x[1].ToLower() == "Hello":
                            {
                                Console.WriteLine("...please wait...");
                                response = ClientTasks.TalkBackHello().Result;
                            }
                            break;
                        case { } x when x[0].ToLower() == "Talkback" && x[1].ToLower() == "Sort":
                            {
                                if (splitInput.Length < 3)
                                {
                                    response = "Please enter a array of integers to sort, e.g. [6,1,8,4,3]";
                                    break;
                                }
                                var integers = splitInput[2].Replace("[", "")
                                    .Replace("]", "")
                                    .Replace(",", "&integers=");
                                Console.WriteLine("...Please wait...");
                                response = ClientTasks.TalkBackSort(integers).Result;
                            }
                            break;
                        case { } x when x[0].ToLower() == "User" && x[1].ToLower() == "Get":
                            {
                                if (splitInput.Length != 3)
                                {
                                    response = "Please enter a Username.";
                                    break;
                                }
                                Console.WriteLine("...Please wait...");
                                response = ClientTasks.UserGet(splitInput[2]).Result;
                            }
                            break;
                        case { } x when x[0].ToLower() == "User" && x[1].ToLower() == "Post":
                            {
                                if (splitInput.Length != 3)
                                {
                                    response = "Please enter a Username.";
                                    break;
                                }
                                Console.WriteLine("....Please wait....");
                                response = ClientTasks.UserPost(splitInput[2]).Result;
                            }
                            break;
                        case { } x when x[0].ToLower() == "User" && x[1].ToLower() == "Set":
                            {
                                if (splitInput.Length != 4)
                                {
                                    response = "Please enter a Username and Api key.";
                                    break;
                                }
                                Console.WriteLine("....Please wait....");
                                ClientTasks.UserSet(splitInput[2], splitInput[3]);
                                response = "Stored";
                            }
                            break;
                        case { } x when x[0].ToLower() == "User" && x[1].ToLower() == "Delete":
                            {
                                Console.WriteLine("....Please wait....");
                                response = ClientTasks.UserDelete().Result.ToString();
                            }
                            break;
                        case { } x when x[0].ToLower() == "User" && x[1].ToLower() == "Role":
                            {
                                if (splitInput.Length != 4)
                                {
                                    response = "Please enter a username and role.";
                                    break;
                                }
                                Console.WriteLine("....Please wait....");
                                response = ClientTasks.ChangeUserRole(splitInput[2], splitInput[3]).Result;
                            }
                            break;
                        case { } x when x[0].ToLower() == "Protected" && x[1].ToLower() == "Hello":
                            {
                                Console.WriteLine("...Please wait...");
                                response = ClientTasks.ProtectedHello().Result;
                            }
                            break;
                        case { } x when x[0].ToLower() == "Protected" && x[1].ToLower() == "Sha1":
                            {
                                if (splitInput.Length < 3)
                                {
                                    response = "Please provide a message.";
                                    break;
                                }
                                var message = new StringBuilder();
                                for (var i = 2; i < splitInput.Length; i++) message.Append(splitInput[i]).Append(" ");
                                Console.WriteLine("....Please wait....");
                                response = ClientTasks.ProtectedSha1(message.ToString().Trim()).Result;
                            }
                            break;
                        case { } x when x[0].ToLower() == "Protected" && x[1].ToLower() == "Sha256":
                            {
                                if (splitInput.Length < 3)
                                {
                                    response = "Please provide a message.";
                                    break;
                                }
                                var message = new StringBuilder();
                                for (var i = 2; i < splitInput.Length; i++) message.Append(splitInput[i]).Append(" ");
                                Console.WriteLine("....Please wait....");
                                response = ClientTasks.ProtectedSha256(message.ToString().Trim()).Result;
                            }
                            break;
                        case { } x when x[0].ToLower() == "Protected" && x[1].ToLower() == "Get" &&
                                        x[2].ToLower() == "Publickey":
                            {
                                Console.WriteLine("....Please wait....");
                                response = ClientTasks.GetPublicKey().Result;
                            }
                            break;
                        case { } x when x[0].ToLower() == "protected" && x[1].ToLower() == "sign":
                            {
                                if (splitInput.Length < 3)
                                {
                                    response = "Please provide a message.";
                                    break;
                                }
                                var message = new StringBuilder();
                                for (var i = 2; i < splitInput.Length; i++) message.Append(splitInput[i]).Append(" ");
                                Console.WriteLine("....please wait....");
                                response = ClientTasks.ProtectedSign(message.ToString().Trim()).Result;
                            }
                            break;
                        case { } x when x[0].ToLower() == "Protected" && x[1].ToLower() == "AddFifty":
                            {
                                if (splitInput.Length != 3)
                                {
                                    response = "Please provide a number.";
                                    break;
                                }
                                if (!int.TryParse(splitInput[2], out var o))
                                {
                                    response = "A valid integer must be given!";
                                    break;
                                }
                                Console.WriteLine("....Please wait....");
                                response = ClientTasks.ProtectedAddFifty(splitInput[2]).Result;
                            }
                            break;
                        default:
                            {
                                Console.WriteLine("Command not recognised.");
                            }
                            break;
                    }
                    if (!string.IsNullOrWhiteSpace(response))
                    {
                        Console.WriteLine(response);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.GetBaseException().Message);
                }
                Console.WriteLine("What would you like to do next?");
                userInput = Console.ReadLine();
                Console.Clear();
            }
            Environment.Exit(1);
        }
    }
    #endregion
}
