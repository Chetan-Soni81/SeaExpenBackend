namespace SeaExpenBackend.Services
{
    public interface IRepository
    {
        public int Create(object o);
        public int Update(object o);
        public int Delete(int id);
        public object GetById(int id);
        public object GetAll();
    }
}
