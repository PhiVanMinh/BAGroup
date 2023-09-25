using Grpc.Core;
using ReportSpeedOver.API.Protos;
using System.Threading.Tasks;
using VehicleInformation.Interfaces.IService;

using ReportDataService = ReportSpeedOver.API.Protos.ReportDataService;

namespace ReportSpeedOver.API.Services
{
    public class ReportSpeedOverVehicleService : ReportDataService.ReportDataServiceBase
    {
        private readonly IActivitySummariesService _activitySummaries;
        private readonly ISpeedOversService _speedOvers;
        private readonly ITransportTypesService _transportTypes;
        private readonly IVehiclesService _vehicle;
        private readonly IVehicleTransportTypesService _vhcTransportTypes;

        public ReportSpeedOverVehicleService(
            IActivitySummariesService activitySummaries,
            ISpeedOversService speedOvers,
            ITransportTypesService transportTypes,
            IVehiclesService vehicle,
            IVehicleTransportTypesService vhcTransportTypes
        )
        {
            _activitySummaries = activitySummaries;
            _speedOvers = speedOvers;
            _transportTypes = transportTypes;
            _vehicle = vehicle;
            _vhcTransportTypes = vhcTransportTypes;
        }

        //public async override Task<ActivitySummaries> GetActivitySummaries(GetById request, ServerCallContext context)
        //{
        //    var offersData = await _activitySummaries.GetAllByCompany(request.Id);
        //    ActivitySummaries response = new ActivitySummaries();
        //    foreach (Report_ActivitySummaries offer in offersData)
        //    {
        //        response.Items.Add(_mapper.Map<OfferDetail>(offer));
        //    }
        //    return response;
        //}
    }
}
