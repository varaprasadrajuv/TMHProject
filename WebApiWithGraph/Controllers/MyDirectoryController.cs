using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApiWithGraph.Models;
using WebApiWithGraph.Services;
using Microsoft.Graph;
using System.Net;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;
using System.Net.Http;

namespace WebApiWithGraph.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyDirectoryController : ControllerBase
    {
        internal static class RouteNames
        {
            public const string Users = nameof(Users);
            public const string UserById = nameof(UserById);
            public const string Groups = nameof(Groups);
            public const string GroupById = nameof(GroupById);
        }

        [HttpPost("UpdateUsers")]     
        public async Task<string> UpdateUsers(UserDetails ud)
        {
            Users users = new Users();
           // HttpResponseMessage response = null;
            try
            {
                users.resources = new List<Models.User>();

                // Initialize the GraphServiceClient.

                GraphServiceClient client = await MicrosoftGraphClient.GetGraphServiceClient();

                Microsoft.Graph.User usn = new Microsoft.Graph.User();

                var user = new Microsoft.Graph.User
                {
                    Identities = new List<ObjectIdentity>()
                    {
                    new ObjectIdentity
                    {
                        SignInType = "emailAddress",
                        Issuer = "varaneworg.onmicrosoft.com",
                        IssuerAssignedId = ud.emailid
                    }
                   }

                };
                //var user = new Microsoft.Graph.User
                //{
                //    UserPrincipalName=ud.emailid
                //};
                await client.Users[ud.ObjectId].Request().UpdateAsync(user);
                return "1";

            }
            catch (ServiceException ex)
            {
                return ex.Message;
            }
            //return response;
        }
        [HttpGet("users/{UserPrincipalName}")]
        public async Task<IActionResult> GetUser(string UserPrincipalName)
        {
            Models.User objUser = new Models.User();
            try
            {
                // Initialize the GraphServiceClient.
                GraphServiceClient client = await MicrosoftGraphClient.GetGraphServiceClient();

                // Load user profile.
                var user = await client.Users[UserPrincipalName]
                                     .Request()
                                     .Select("displayName,givenName,postalCode,identities")
                                     .GetAsync();

                // Copy Microsoft-Graph User to DTO User
               objUser = CopyHandler.UserProperty(user);

                return Ok(objUser);
            }
            catch (ServiceException ex)
            {
                if (ex.StatusCode == HttpStatusCode.BadRequest)
                {
                    return BadRequest();
                }
                else
                {
                    return NotFound();
                }
            }
        }


        [HttpGet("users/")]
        public async Task<IActionResult> GetUsers()
        {
            Users users = new Users();
            try
            {
                users.resources = new List<Models.User>();

                // Initialize the GraphServiceClient.
                GraphServiceClient client = await MicrosoftGraphClient.GetGraphServiceClient();

                // Load users profiles.
                var userList = await client.Users
                    .Request()
                    .Select(e => new
                    {
                        e.DisplayName,                       
                        e.Id,
                        e.Identities,
                        e.GivenName,
                        e.Surname,
                        e.UserPrincipalName,
                        e.Mail,
                        e.MailNickname,
                        e.Manager,
                        e.UserType
                    })
                    .GetAsync();

                // Copy Microsoft User to DTO User
                foreach (var user in userList)
                {
                    var objUser = CopyHandler.UserProperty(user);
                    users.resources.Add(objUser);

                }
                //users.totalResults = users.resources.Count;

                return Ok(users);
            }
            catch (ServiceException ex)
            {
                if (ex.StatusCode == HttpStatusCode.BadRequest)
                {
                    return BadRequest();
                }
                else
                {
                    return NotFound();
                }
            }
        }
        //[System.Web.Http.HttpPost]
        //[EnableCors("AllowOrigin")]
        //[System.Web.Http.Route("UpdateUsers/{ObjectId}/{emailid}")]       
        //public async Task<int> UpdateUsers(string ObjectId, string emailid)
        //{
        //    Users users = new Users();
        //    HttpResponseMessage response = null;
        //    try
        //    {
        //        users.resources = new List<Models.User>();

        //        // Initialize the GraphServiceClient.

        //        GraphServiceClient client = await MicrosoftGraphClient.GetGraphServiceClient();

        //        Microsoft.Graph.User usn = new Microsoft.Graph.User();

        //        var user = new Microsoft.Graph.User
        //        {
        //          Identities = new List<ObjectIdentity>()
        //            {
        //            new ObjectIdentity
        //            {
        //                SignInType = "emailAddress",
        //                Issuer = "varasukanyaorg.onmicrosoft.com",
        //                IssuerAssignedId = emailid
        //            }
        //           }

        //        };

        //        await client.Users[ObjectId].Request().UpdateAsync(user);
        //        return 1;

        //    }
        //    catch (ServiceException ex)
        //    {
        //        return 1;
        //    }
        //    //return response;
        //}

        //[HttpGet("groups/{id}", Name = RouteNames.GroupById)]
        //public async Task<IActionResult> GetGroup(string id)
        //{
        //    Models.Group objGroup = new Models.Group();
        //    try
        //    {

        //        // Initialize the GraphServiceClient.
        //        GraphServiceClient client = await MicrosoftGraphClient.GetGraphServiceClient();

        //        // Load group profile.
        //        var group = await client.Groups[id].Request().GetAsync();

        //        // Copy Microsoft-Graph Group to DTO Group
        //        objGroup = CopyHandler.GroupProperty(group);

        //        return Ok(objGroup);
        //    }
        //    catch (ServiceException ex)
        //    {
        //        if (ex.StatusCode == HttpStatusCode.BadRequest)
        //        {
        //            return BadRequest();
        //        }
        //        else
        //        {
        //            return NotFound();
        //        }
        //    }
        //}
        //[HttpPost("Createusers/")]
        //public async Task<IActionResult> CreateUser(Microsoft.Graph.User user)
        //{
        //    GraphServiceClient client = await MicrosoftGraphClient.GetGraphServiceClient();

        //    //Models.User us = new Models.User();

        //    //us.id = graphUser.Id;
        //    //us.givenName = graphUser.GivenName;
        //    //us.surname = graphUser.Surname;
        //    //us.userPrincipalName = graphUser.UserPrincipalName;
        //    //us.email = graphUser.Mail;
        //    //var user = new Microsoft.Graph.User
        //    //{
        //    //    AccountEnabled = true,
        //    //    DisplayName = "varamnew",
        //    //    MailNickname = "VaramNew",
        //    //    Mail="varaprasad204@rediffmail.com",
        //    //    UserPrincipalName = "varaprasad204@varasukanyaorg.onmicrosoft.com",
        //    //    PasswordProfile = new PasswordProfile
        //    //    {
        //    //        ForceChangePasswordNextSignIn = true,
        //    //        Password = "xWwvJ]6NMw+bWH-d"
        //    //    }
        //    //};

        //    var ruser = await client.Users
        //        .Request()
        //        .AddAsync(user);
        //    //return Ok();
        //    return Ok(ruser);
        //}
        //[HttpGet("GetAuth/")]
        //public async Task<IActionResult> getAuthentication()
        //{
        //    GraphServiceClient client = await MicrosoftGraphClient.GetGraphServiceClient();
        //    var microsoftAuthenticatorMethod = await client.Users["{9d80f5ac-e430-4532-89ec-84198b7f774a}"].Authentication.MicrosoftAuthenticatorMethods
        //                                       .Request()
        //                                       .GetAsync();
        //    return Ok(microsoftAuthenticatorMethod);       

        //    }
        //[HttpGet("groups/")]
        //public async Task<IActionResult> GetGroups()
        //{
        //    Groups groups = new Groups();
        //    try
        //    {
        //        groups.resources = new List<Models.Group>();

        //        // Initialize the GraphServiceClient.
        //        GraphServiceClient client = await MicrosoftGraphClient.GetGraphServiceClient();

        //        // Load groups profiles.
        //        var groupList = await client.Groups.Request().GetAsync();

        //        // Copy Microsoft-Graph Group to DTO Group
        //        foreach (var group in groupList)
        //        {
        //            var objGroup = CopyHandler.GroupProperty(group);
        //            groups.resources.Add(objGroup);
        //        }
        //        groups.totalResults = groups.resources.Count;

        //        return Ok(groups);
        //    }
        //    catch (ServiceException ex)
        //    {
        //        if (ex.StatusCode == HttpStatusCode.BadRequest)
        //        {
        //            return BadRequest();
        //        }
        //        else
        //        {
        //            return NotFound();
        //        }
        //    }
        //}

    }
}