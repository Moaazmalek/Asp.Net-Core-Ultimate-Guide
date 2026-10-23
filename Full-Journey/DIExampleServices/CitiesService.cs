namespace DIExampleServices
{
    public class CitiesService:ICitiesService
    {
        private List<string> _cities;
        private Guid _serviceInstanceId;
        public Guid ServiceInstanceId { get
            {
                return _serviceInstanceId;
            } }
        public CitiesService()
        {
            _cities = ["London", "Paris", "New york", "Tokyo", "Rome"];
            _serviceInstanceId = Guid.NewGuid();
        }

        public List<string> GetCities()
        {
            return _cities;
        }


    }
}
