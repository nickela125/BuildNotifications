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

        public VsoClient(IRestClient restClient, IMapper mapper)
        {
            _restClient = restClient;
            _mapper = mapper;
        }

        public async Task<IList<VsoProject>> GetProjects(VsoAccount account)
        {
            _restClient.SetBasicCredentials(account.EncodedCredentials);
            string address = VsoAddressHelper.GetFullAddress(account,  Constants.VsoProjectsAddress);
            ProjectList projectList = await _restClient.GetRequest<ProjectList>(null, address);
            return projectList.Value.Select(project => _mapper.MapToVsoProject(project)).ToList();
        }

        public async Task<List<VsoBuild>> GetBuilds(VsoProject project, VsoAccount account)
        {
            _restClient.SetBasicCredentials(account.EncodedCredentials);
            string address = VsoAddressHelper.GetFullAddress(account, string.Format(Constants.VsoBuildDefinitionsAddress, project.Id));
            Model.DTO.BuildList buildList = await _restClient.GetRequest<Model.DTO.BuildList>(null, address);
            return buildList.Value.Select(build => _mapper.MapToVsoBuild(build)).ToList();
        }
    }
}
