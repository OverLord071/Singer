namespace Singer.Domain;

public class Alias
{
    public string AliasValue { get; }
    public string Name { get; }

    public Alias(string alias, string name)
    {
        AliasValue = alias;
        Name = name;
    }

    public override string ToString()
    {
        return Name;
    }

    public override int GetHashCode()
    {
        int hash = 17;
        hash = hash * 23 + (AliasValue != null ? AliasValue.GetHashCode() : 0);
        hash = hash * 23 + (Name != null ? Name.GetHashCode() : 0);
        return hash;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        Alias other = (Alias)obj;
        return (AliasValue == other.AliasValue || (AliasValue != null && AliasValue.Equals(other.AliasValue))) &&
               (Name == other.Name || (Name != null && Name.Equals(other.Name)));
    }
}

