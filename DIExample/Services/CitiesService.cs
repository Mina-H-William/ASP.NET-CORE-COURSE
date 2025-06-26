using ServiceContracts;

namespace Services
{
    public class CitiesService : ICitiesService, IDisposable
    {
        private List<string> _cities;

        private Guid _ServiceInstanceId;

        public Guid ServiceInstanceId
        {
            get
            {
                return _ServiceInstanceId;
            }
        }
        public CitiesService()
        {
            _ServiceInstanceId = Guid.NewGuid();

            _cities = new List<string>()
            {
                "London",
                "Paris",
                "New York",
                "Tokyo"
            };
        }

        public List<string> GetCities()
        {
            return _cities;
        }

        // used to do some logic when object will distroy
        public void Dispose()
        {
            // to do: add logic to close Connection with database
        }
    }
}
