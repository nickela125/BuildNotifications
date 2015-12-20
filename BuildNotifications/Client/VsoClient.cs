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

        // TODO fix up how credentials are passed in
        public VsoClient(IRestClient restClient, IMapper mapper)
        {
            _restClient = restClient;
            _mapper = mapper;
        }

        public async Task<IList<VsoProject>> GetProjects(string accountName, string encodedCredentials)
        {
            _restClient.SetBasicCredentials(encodedCredentials);
            string address = VsoAddressHelper.GetFullAddress(accountName,  Constants.VsoProjectsAddress);
            ProjectList projectList = await _restClient.GetRequest<ProjectList>(null, address);
            return projectList.Value.Select(project => _mapper.MapToVsoProject(project)).ToList();
        }

        public async Task<IList<VsoBuildDefinition>> GetBuildDefinitions(VsoProject project, string accountName, string encodedCredentials)
        {
            _restClient.SetBasicCredentials(encodedCredentials);
            string address = VsoAddressHelper.GetFullAddress(accountName, string.Format(Constants.VsoBuildDefinitionsAddress, project.Id));
            BuildDefinitionList buildList = await _restClient.GetRequest<BuildDefinitionList>(null, address);
            return buildList.Value.Select(build => _mapper.MapToVsoBuildDefinition(build)).ToList();
        }

        public async Task<IList<VsoBuild>> GetBuilds(VsoProject project, string accountName, string encodedCredentials, IList<string> buildDefinitions)
        {
            _restClient.SetBasicCredentials(encodedCredentials);
            string formattedBuildDefinitions = string.Join(",", buildDefinitions);
            
            string address = VsoAddressHelper.GetFullAddress(accountName, string.Format(Constants.VsoBuildsAddress, project.Id, formattedBuildDefinitions, NumberOfBuildsToCheck));
            Model.DTO.BuildList buildList = await _restClient.GetRequest<Model.DTO.BuildList>(null, address);
            return buildList.Value.Select(record => _mapper.MapToVsoBuild(record)).ToList();
        }
    }
}
