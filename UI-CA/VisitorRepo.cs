namespace UI;

public class VisitorRepo
{
    private List<Visitor> visitors;

    public VisitorRepo(List<Visitor> visitors)
    {
        this.visitors = visitors;
    }

    public List<Visitor> GetAllVisitors()
    {
        return visitors;
    }

    public List<Visitor> GetVisitorsByNameOrCity(string firstName, string city)
    {
        return visitors.Where(v =>
            (string.IsNullOrEmpty(firstName) || v.FirstName.ToLower().Contains(firstName.ToLower())) &&
             (string.IsNullOrEmpty(city) || v.City.ToLower().Contains(city.ToLower()))
            ).ToList();
    }
}