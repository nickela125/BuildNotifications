using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildNotifications.Helpers;
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

        public async Task<IList<VsoProject>> GetProjects(AccountDetails accountDetails)
        {
            _restClient.SetBasicCredentials(accountDetails.EncodedCredentials);
            string address = VsoAddressHelper.GetFullAddress(accountDetails.AccountName,  Constants.VsoProjectsAddress);
            ProjectList projectList = await _restClient.GetRequest<ProjectList>(null, address);
            return projectList.Value.Select(project => _mapper.MapToVsoProject(project)).ToList();
        }

        public async Task<IList<VsoBuildDefinition>> GetBuildDefinitions(AccountDetails accountDetails)
        {
            _restClient.SetBasicCredentials(accountDetails.EncodedCredentials);
            string address = VsoAddressHelper.GetFullAddress(accountDetails.AccountName, string.Format(Constants.VsoBuildDefinitionsAddress, accountDetails.ProjectId));
            BuildDefinitionList buildList = await _restClient.GetRequest<BuildDefinitionList>(null, address);
            return buildList.Value.Select(build => _mapper.MapToVsoBuildDefinition(build)).ToList();
        }

        public async Task<IList<VsoBuild>> GetBuilds(AccountDetails accountDetails, IList<string> buildDefinitions)
        {
            _restClient.SetBasicCredentials(accountDetails.EncodedCredentials);
            string formattedBuildDefinitions = string.Join(",", buildDefinitions);
            
            string address = VsoAddressHelper.GetFullAddress(accountDetails.AccountName, string.Format(Constants.VsoBuildsAddress, accountDetails.ProjectId, formattedBuildDefinitions, NumberOfBuildsToCheck));
            Model.DTO.BuildList buildList = await _restClient.GetRequest<Model.DTO.BuildList>(null, address);
            return buildList.Value.Select(record => _mapper.MapToVsoBuild(record)).ToList();
        }
    }
}
