using System.Reflection;
using Joseph.Data.Entities;
using Joseph.Enums;
using TypeFilter = Joseph.Enums.TypeFilter;

namespace Joseph.WebApi.Utils;

public static class FilteringEntities
{
    public static List<Job> FilterJobs(List<Job> jobs, Filter[] filters)
    {
        PropertyInfo prop;
        foreach (Filter filter in filters)
        {
            prop = typeof(Job).GetProperty(filter.Property);
            
            if (prop is null) continue;
            if (filter.TypeFilter == TypeFilter.CheckBox)
            {
                if (prop.PropertyType.IsClass && prop.PropertyType != typeof(String))
                {
                    Func<Job, bool> func = job =>
                    {
                        Type typeProperty = prop.PropertyType;
                        dynamic value = Convert.ChangeType(prop.GetValue(job), typeProperty);

                        for (int i = 0; i < value.Count; i++)
                        {
                            Type p = value[i].GetType();
                            PropertyInfo internalProperty = p.GetProperty("Name");
                            Type internalTypeProperty = internalProperty.PropertyType;
                            object internalValue = Convert.ChangeType(internalProperty.GetValue(value[i]), internalTypeProperty);
                    
                            if (internalValue is null) return false;
                            foreach (string filterValue in filter.Values)
                            {
                                if ((string) internalValue == filterValue) return true;
                            }
                        }

                        return false;
                    };
                    jobs = jobs.Where(func).ToList();
                }
                else
                {
                    Func<Job, bool> func = job =>
                    {
                        Type typeProperty = prop.PropertyType;
                        object value = Convert.ChangeType(prop.GetValue(job), typeProperty);

                        if (value is null) return false;
                        foreach (string filterValue in filter.Values)
                        {
                            if ((string) value == filterValue) return true;
                        }

                        return false;
                    };
                    jobs = jobs.Where(func).ToList();
                }
            }
            else if (filter.TypeFilter == TypeFilter.Enums)
            {
                Func<Job, bool> func = job =>
                {
                    Type typeProperty = prop.PropertyType;
                    object value  = Convert.ChangeType(prop.GetValue(job), typeProperty);
                    
                    if (value is null) return false;
                    foreach (string filterValue in filter.Values)
                    {
                        if ((NumberOfHiredPeople) value == Enum.Parse<NumberOfHiredPeople>(filterValue)) return true;
                    }

                    return false;
                };
                jobs = jobs.Where(func).ToList();
            }
            else if (filter.TypeFilter == TypeFilter.MinMax)
            {
                bool Func(Job job)
                {
                    Type typeProperty = prop.PropertyType;
                    object value = Convert.ChangeType(prop.GetValue(job), typeProperty);

                    int? min = null, max = null;
                    if (filter.Values is not null && filter.Values.Count == 2)
                    {
                        if (filter.Values[0] is not null)
                        {
                            min = Convert.ToInt32(filter.Values[0]);
                        }

                        if (filter.Values[1] is not null)
                        {
                            max = Convert.ToInt32(filter.Values[1]);
                        }
                    }

                    int intValue = (int) value;

                    if (min is not null && max is not null)
                    {
                        if (intValue >= min && intValue <= max) return true;
                    }

                    if (min is not null && max is null)
                    {
                        if (intValue >= min) return true;
                    }

                    if (max is not null && min is null)
                    {
                        if (intValue <= max) return true;
                    }

                    return false;
                }

                jobs = jobs.Where((Func<Job, bool>) Func).ToList();
            }
        }

        return jobs;
    }
}