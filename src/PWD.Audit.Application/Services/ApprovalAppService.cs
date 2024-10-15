using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PWD.Audit.DtoModels;
using PWD.Audit.Interfaces;
using PWD.Audit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Repositories;

namespace PWD.Audit.Services
{
    [Authorize]
    public class ApprovalAppService : ApplicationService, IApprovalAppService
    {
        string clientUrl = PermissionHelper._identityClientUrl;
        string authUrl = PermissionHelper._authority;
        private readonly ILogger _logger;

        private readonly IRepository<Posting, int> _repository;

        public ApprovalAppService(IRepository<Posting, int> repository, ILogger<ApprovalAppService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        private async Task<TokenResponse> GetToken()
        {
            var authorityUrl = $"{PermissionHelper._authority}";

            var authority = new HttpClient();
            var discoveryDocument = await authority.GetDiscoveryDocumentAsync(authorityUrl);
            if (discoveryDocument.IsError)
            {
                //return null;
            }

            // Request Token
            var tokenResponse = await authority.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = PermissionHelper._clientId,
                ClientSecret = PermissionHelper._clientSecret,
                Scope = PermissionHelper._scope
            });

            if (tokenResponse.IsError)
            {
                //return null;
            }
            return tokenResponse;
        }

        [AllowAnonymous]
        public async Task<PostingDto> GetPosting(string userName)
        {
            if (_repository.Any(p => p.UserName == userName))
            {
                var user = _repository.OrderBy(x => x.PostingId).Last(p => p.UserName == userName);
                return ObjectMapper.Map<Posting, PostingDto>(user);
            }
            using (var client = new HttpClient())
            {


                var tokenResponse = await GetToken();
                client.BaseAddress = new Uri(clientUrl);
                client.SetBearerToken(tokenResponse.AccessToken);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //GET Method
                HttpResponseMessage response =
                    await client.GetAsync($"api/app/posting/user-info?userName={userName}");
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var postString = await response.Content.ReadAsStringAsync();
                        var post = JsonSerializer.Deserialize<PostingConsumeDto>(postString);
                        var result = new PostingDto()
                        {
                            Post = post.post,
                            Designation = post.designation,
                            DesignationBn = post.designationBn,
                            EmployeeId = post.employeeId,
                            UserId = post.id,
                            Name = post.name,
                            NameBn = post.nameBn,
                            Office = post.office,
                            OfficeBn = post.officeBn,
                            PostingId = post.postingId,
                            OrgUniId = post.orgUniId,
                            UserName = post.userName,
                        };
                        var postInput = ObjectMapper.Map<PostingDto, Posting>(result);
                        if (post != null && post.userName != null)
                            await _repository.InsertAsync(postInput);
                        return result;
                        // put the code here that may raise exceptions
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Internal server Error");
                }
            }
            return new PostingDto();
        }

        public async Task<PostingDto> UpdatePosting(string userName)
        {

            using (var client = new HttpClient())
            {
                var tokenResponse = await GetToken();
                client.BaseAddress = new Uri(clientUrl);
                client.SetBearerToken(tokenResponse.AccessToken);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //GET Method
                HttpResponseMessage response =
                    await client.GetAsync($"api/app/posting/user-info?userName={userName}");
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var postString = await response.Content.ReadAsStringAsync();
                        var post = JsonSerializer.Deserialize<PostingConsumeDto>(postString);
                        var result = new PostingDto()
                        {
                            Post = post.post,
                            Designation = post.designation,
                            DesignationBn = post.designationBn,
                            EmployeeId = post.employeeId,
                            UserId = post.id,
                            Name = post.name,
                            NameBn = post.nameBn,
                            Office = post.office,
                            OfficeBn = post.officeBn,
                            PostingId = post.postingId,
                            OrgUniId = post.orgUniId,
                            UserName = post.userName,
                        };
                        var postInput = ObjectMapper.Map<PostingDto, Posting>(result);
                        if (post != null && post.userName != null && !_repository.Any(x => x.PostingId == post.postingId))
                            await _repository.InsertAsync(postInput);
                        return result;
                        // put the code here that may raise exceptions
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Internal server Error");
                }
            }
            return new PostingDto();
        }

        [AllowAnonymous]
        public async Task<PostingDto> GetPostingById(int id)
        {

            using (var client = new HttpClient())
            {
                var tokenResponse = await GetToken();
                client.BaseAddress = new Uri(clientUrl);
                client.SetBearerToken(tokenResponse.AccessToken);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //GET Method
                HttpResponseMessage response =
                    await client.GetAsync($"api/app/posting/{id}/posting-info");
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var postString = await response.Content.ReadAsStringAsync();
                        var post = JsonSerializer.Deserialize<PostingConsumeDto>(postString);
                        //var result = ObjectMapper.Map<PostingConsumeDto, PostingDto>(post);
                        var result = new PostingDto()
                        {
                            Post = post.post,
                            Designation = post.designation,
                            DesignationBn = post.designationBn,
                            EmployeeId = post.employeeId,
                            UserId = post.id,
                            Name = post.name,
                            NameBn = post.nameBn,
                            Office = post.office,
                            OfficeBn = post.officeBn,
                            PostingId = post.postingId,
                            OrgUniId = post.orgUniId,
                            UserName = post.userName,
                        };
                        return result;
                        // put the code here that may raise exceptions
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Internal server Error");
                }
            }
            return new PostingDto();
        }
        [AllowAnonymous]
        public async Task<List<PostingDto>> GetPostingListById(List<int> ids)
        {
            var idString = "";
            ids.ForEach(i => idString += "idList=" + i + "&");
            idString = idString.Remove(idString.Length - 1);

            using (var client = new HttpClient())
            {
                var tokenResponse = await GetToken();
                client.BaseAddress = new Uri(clientUrl);
                client.SetBearerToken(tokenResponse.AccessToken);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //GET Method
                HttpResponseMessage response =
                    await client.GetAsync($"api/app/posting/posting-list?{idString}");
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var postString = await response.Content.ReadAsStringAsync();
                        var posts = JsonSerializer.Deserialize<List<PostingConsumeDto>>(postString);
                        //var resultList = ObjectMapper.Map<List<PostingConsumeDto>, List<PostingDto>>(posts); ;

                        var resultList = new List<PostingDto>();
                        posts.ForEach(post =>
                        {
                            var result = new PostingDto()
                            {
                                Post = post.post,
                                Designation = post.designation,
                                DesignationBn = post.designationBn,
                                EmployeeId = post.employeeId,
                                UserId = post.id,
                                Name = post.name,
                                NameBn = post.nameBn,
                                Office = post.office,
                                OfficeBn = post.officeBn,
                                PostingId = post.postingId,
                                OrgUniId = post.orgUniId,
                                UserName = post.userName,
                            };
                            resultList.Add(result);
                        });

                        return resultList;
                        // put the code here that may raise exceptions
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Internal server Error");
                }
            }
            return new List<PostingDto>();
        }

        [AllowAnonymous]
        [HttpGet]
        [DisableAuditing]
        public async Task<DateTime> LatestOffice()
        {
            using (var client = new HttpClient())
            {
                var tokenResponse = await GetToken();
                client.BaseAddress = new Uri(clientUrl);
                client.SetBearerToken(tokenResponse.AccessToken);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //GET Method
                HttpResponseMessage response =
                    await client.GetAsync($"api/app/organization-unit/latest");
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var postString = await response.Content.ReadAsStringAsync();
                        var post = JsonSerializer.Deserialize<DateTime>(postString);
                        return post;
                        // put the code here that may raise exceptions
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Internal server Error");
                }
            }
            return new DateTime();
        }
        [AllowAnonymous, HttpGet]

        public async Task<List<OrganizationUnitDto>> GetOffices()
        {
            using (var client = new HttpClient())
            {
                var tokenResponse = await GetToken();
                client.BaseAddress = new Uri(clientUrl);
                client.SetBearerToken(tokenResponse.AccessToken);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var offices = new List<OrganizationUnitDto>();
                var roles = new List<RoleConsumeDto>();
                //GET Method
                HttpResponseMessage response =
                    await client.GetAsync($"api/app/organization-unit/offices");
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var postString = await response.Content.ReadAsStringAsync();
                        offices = JsonSerializer.Deserialize<List<OrganizationUnitDto>>(postString);
                        HttpResponseMessage responseRole = await client.GetAsync($"api/app/organization-unit/roles");

                        if (responseRole.IsSuccessStatusCode)
                        {
                            try
                            {
                                var roleString = await responseRole.Content.ReadAsStringAsync();
                                roles = JsonSerializer.Deserialize<List<RoleConsumeDto>>(roleString);
                                offices.ForEach(o =>
                                {
                                    o.roles.ForEach(r =>
                                    {
                                        o.roleNames.Add(roles.FirstOrDefault(x => x.id == r.roleId)?.name);
                                    });
                                    o.roles = null;

                                });

                                return offices;
                                // put the code here that may raise exceptions
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Internal server Error");
                        }
                        return offices;
                        // put the code here that may raise exceptions
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Internal server Error");
                }


            }
            return new List<OrganizationUnitDto>();
        }

        public async Task<UserInfo> GetUserInfo(string userName)
        {
            if (_repository.Any(p => p.UserName == userName))
            {
                var user = _repository.First(p => p.UserName == userName);
                return new UserInfo()
                {
                    name = user.Name,
                    id = user.UserId,
                    userName = user.UserName,                    
                };
            }
            using (var client = new HttpClient())
            {
                var tokenResponse = await GetToken();
                client.BaseAddress = new Uri(clientUrl);
                client.SetBearerToken(tokenResponse.AccessToken);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var user = new UserInfo();
                //GET Method

                HttpResponseMessage response =
                    await client.GetAsync($"api/app/posting/user-by-name?userName={userName}");
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        user = JsonSerializer.Deserialize<UserInfo>(responseString);
                        return user;
                        // put the code here that may raise exceptions
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Internal server Error");
                }


            }
            return new UserInfo();
        }
        public async Task<UserInfo> GetUserInfoById(string userId)

        {
            if (_repository.Any(p => p.UserId == Guid.Parse(userId)))
            {
                var user = _repository.First(p => p.UserId == Guid.Parse(userId));
                return new UserInfo()
                {
                    name = user.Name,
                    id = user.UserId,
                    userName = user.UserName,
                };
            }
            using (var client = new HttpClient())
            {
                var tokenResponse = await GetToken();
                client.BaseAddress = new Uri(clientUrl);
                client.SetBearerToken(tokenResponse.AccessToken);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var user = new UserInfo();
                //GET Method

                HttpResponseMessage response =
                    await client.GetAsync($"api/app/posting/user/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        user = JsonSerializer.Deserialize<UserInfo>(responseString);
                        return user;
                        // put the code here that may raise exceptions
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Internal server Error");
                }


            }
            return new UserInfo();
        }

        [HttpGet]
        public async Task<List<ColleagueDto>> OfficeUsers(string userName)

        {
            using (var client = new HttpClient())
            {
                var tokenResponse = await GetToken();
                client.BaseAddress = new Uri(clientUrl);
                client.SetBearerToken(tokenResponse.AccessToken);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var users = new List<ColleagueDto>();
                //GET Method

                HttpResponseMessage response =
                    await client.GetAsync($"api/app/organization-unit/office-users?userName={userName}");
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    _logger.Log(LogLevel.Information, responseString);
                    users = JsonSerializer.Deserialize<List<ColleagueDto>>(responseString);
                    _logger.Log(LogLevel.Information, JsonSerializer.Serialize(users));

                    return users;

                }
                else
                {
                    return new List<ColleagueDto>();
                }
            }
        }

        [HttpGet]
        public async Task<List<PostingConsumeDto>> OfficePostings(string userName)

        {
            using (var client = new HttpClient())
            {
                var tokenResponse = await GetToken();
                client.BaseAddress = new Uri(clientUrl);
                client.SetBearerToken(tokenResponse.AccessToken);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var postings = new List<PostingConsumeDto>();
                //GET Method

                HttpResponseMessage response =
                    await client.GetAsync($"api/app/organization-unit/office-posts?userName={userName}");
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    _logger.Log(LogLevel.Information, responseString);
                    postings = JsonSerializer.Deserialize<List<PostingConsumeDto>>(responseString);
                    _logger.Log(LogLevel.Information, JsonSerializer.Serialize(postings));

                    return postings;
                }
                else
                {
                    return new List<PostingConsumeDto>();
                }


            }
        }

        [HttpPost]
        public async Task<bool> UpdatePassword(ChangePass input)
        {
            using (var client = new HttpClient())
            {
                var tokenResponse = await GetToken();
                client.BaseAddress = new Uri(clientUrl);
                client.SetBearerToken(tokenResponse.AccessToken);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //GET Method

                var update = JsonSerializer.Serialize(input);
                var requestContent = new StringContent(update, Encoding.UTF8, "application/json");

                HttpResponseMessage response =
                    await client.PostAsync(($"api/app/posting/update-password"), requestContent);
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        return JsonSerializer.Deserialize<bool>(responseString);
                        // put the code here that may raise exceptions
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Internal server Error");
                    return false;
                }
            }

            return false;
        }


    }



}
