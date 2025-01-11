using MongoDB.Driver;

namespace ApiMongoDB
{
    public class MongoDbContext
    {
        private readonly IMongoClient _mongoClient;
        private IClientSessionHandle? _session;
        //private readonly TransactionOptions _transactionOptions;

        public MongoDbContext(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;

            //_transactionOptions = new TransactionOptions(
            //        readConcern: ReadConcern.Local,
            //        writeConcern: WriteConcern.WMajority,
            //        readPreference: ReadPreference.Primary,
            //        maxCommitTime: TimeSpan.FromMinutes(5)
            //);
        }

        public IClientSessionHandle Session //=> _session ?? InitSessionAsync().Result;
        {
            get
            {
                if (_session is null)
                    _session = InitSessionAsync().Result;
                return _session;
            }
        }

        public async Task StartMongoSessionAsync()
        {
            if (_session is null)
                _session = await InitSessionAsync();
            //_session.StartTransaction(_transactionOptions);
            _session.StartTransaction();
        }

        public async Task<IClientSessionHandle> InitSessionAsync()
        {
            //Creamos un método extra para los repositorios de lectura que no están en una trnasacción.
            return await _mongoClient.StartSessionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_session != null)
            {
                await _session.CommitTransactionAsync();
                _session.Dispose();
                _session = null;
            }
        }

        public async Task AbortTransactionAsync()
        {
            if (_session != null)
            {
                await _session.AbortTransactionAsync();
                _session.Dispose();
                _session = null;
            }
        }
    }
}
