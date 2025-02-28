using System.Text.Json;

namespace AspNetMvcExample.Models.Services;

public class EntityService<T> where T : class
{
    public EntityService(IConfiguration configuration)
    {
        var type = typeof(T).Name;
        dataFile = configuration.GetSection($"EntityServices:{type}").Value;
        Load();
    }

    // TODO
    private string dataFile;

    private List<T> Models { get; set; } = [];

    public void Load()
    {
        if (File.Exists(dataFile))
        {
            Models = JsonSerializer.Deserialize<List<T>>(File.ReadAllText(dataFile));
        }
    }

    public List<T> GetAll()
    {
        return Models;
    }

    public void Add(T model)
    {
        Models.Add(model);
    }

    public void Delete(T model)
    {
        Models.Remove(model);
    }


    //public T? FindById(int id)
    //{
    //    return Models.FirstOrDefault(x => x.Id == id);
    //}

    public void SaveChanges()
    {
        File.WriteAllText(dataFile, JsonSerializer.Serialize(Models));
    }
}
