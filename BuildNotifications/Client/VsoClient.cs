using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildNotifications.Common;
using BuildNotifications.Common.Helpers;
using BuildNotifications.Interface.Client;
using BuildNotifications.Interface.Service;
using BuildNotifications.Model;
using BuildNotifications.Model.DTO;

namespace BuildNotifications.Client
{
    public class VsoClient : IVsoClient
    {
        private readonly IRestClient _restClient;
        private readonly IMapper _mapper;
        private const int NumberOfBuildsToCheck = 2;
        
        public VsoClient(IRestClient restClient, IMapper mapper)
        {
            _restClient = restClient;
            _mapper = mapper;
        }

        public async Task<IList<Project>> GetProjects(AccountDetails accountDetails)
        {
            _restClient.SetBasicCredentials(accountDetails.EncodedCredentials);
            string address = VsoAddressHelper.GetFullAddress(accountDetails.AccountName,  Constants.VsoProjectsAddress);
            VsoProjectList projectList = await _restClient.GetRequest<VsoProjectList>(null, address);
            return projectList.Value.Select(project => _mapper.MapToProject(project)).ToList();
        }

        public async Task<IList<BuildDefinition>> GetBuildDefinitions(AccountDetails accountDetails)
        {
            _restClient.SetBasicCredentials(accountDetails.EncodedCredentials);
            string address = VsoAddressHelper.GetFullAddress(accountDetails.AccountName, string.Format(Constants.VsoBuildDefinitionsAddress, accountDetails.ProjectId));
            VsoBuildDefinitionList buildList = await _restClient.GetRequest<VsoBuildDefinitionList>(null, address);
            return buildList.Value.Select(build => _mapper.MapToBuildDefinition(build)).ToList();
        }

        public async Task<IList<Model.Build>> GetBuilds(AccountDetails accountDetails, IList<string> buildDefinitions)
        {
            _restClient.SetBasicCredentials(accountDetails.EncodedCredentials);
            string formattedBuildDefinitions = string.Join(",", buildDefinitions);
            
            string address = VsoAddressHelper.GetFullAddress(accountDetails.AccountName, string.Format(Constants.VsoBuildsAddress, accountDetails.ProjectId, formattedBuildDefinitions, NumberOfBuildsToCheck));
            Model.DTO.VsoBuildList buildList = await _restClient.GetRequest<Model.DTO.VsoBuildList>(null, address);
            return buildList.Value.Select(record => _mapper.MapToBuild(record)).ToList();
        }
    }
}
