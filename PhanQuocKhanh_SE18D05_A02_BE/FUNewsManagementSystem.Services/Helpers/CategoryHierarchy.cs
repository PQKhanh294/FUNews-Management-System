using FUNewsManagementSystem.BusinessObjects.Entities;
using FUNewsManagementSystem.Services.Dtos;

namespace FUNewsManagementSystem.Services.Helpers;

public static class CategoryHierarchy
{
    public static bool WouldCreateCycle(
        IReadOnlyCollection<Category> categories,
        short categoryId,
        short? proposedParentId)
    {
        if (proposedParentId is null)
        {
            return false;
        }

        var parentById = categories.ToDictionary(c => c.CategoryID, c => c.ParentCategoryID);
        var visited = new HashSet<short>();
        var current = proposedParentId;

        while (current is not null)
        {
            if (current == categoryId || !visited.Add(current.Value))
            {
                return true;
            }

            current = parentById.GetValueOrDefault(current.Value);
        }

        return false;
    }

    public static List<CategoryTreeDto> BuildTree(IReadOnlyCollection<Category> categories)
    {
        var ordered = categories.OrderBy(c => c.CategoryName).ToList();
        var knownIds = ordered.Select(c => c.CategoryID).ToHashSet();
        var childrenByParent = ordered
            .Where(c => c.ParentCategoryID is not null && c.ParentCategoryID != c.CategoryID)
            .GroupBy(c => c.ParentCategoryID!.Value)
            .ToDictionary(group => group.Key, group => group.ToList());
        var visited = new HashSet<short>();
        var result = new List<CategoryTreeDto>();

        var roots = ordered.Where(c =>
            c.ParentCategoryID is null ||
            c.ParentCategoryID == c.CategoryID ||
            !knownIds.Contains(c.ParentCategoryID.Value));

        foreach (var root in roots)
        {
            var node = BuildNode(root, childrenByParent, visited, []);
            if (node is not null)
            {
                result.Add(node);
            }
        }

        // Legacy data may already contain a cycle with no root. Keep the menu
        // usable while truncating the repeated edge instead of recursing forever.
        foreach (var category in ordered.Where(c => !visited.Contains(c.CategoryID)))
        {
            var node = BuildNode(category, childrenByParent, visited, []);
            if (node is not null)
            {
                result.Add(node);
            }
        }

        return result;
    }

    private static CategoryTreeDto? BuildNode(
        Category category,
        IReadOnlyDictionary<short, List<Category>> childrenByParent,
        ISet<short> visited,
        HashSet<short> path)
    {
        if (visited.Contains(category.CategoryID) || !path.Add(category.CategoryID))
        {
            return null;
        }

        visited.Add(category.CategoryID);
        var children = new List<CategoryTreeDto>();
        if (childrenByParent.TryGetValue(category.CategoryID, out var childEntities))
        {
            foreach (var child in childEntities)
            {
                var childNode = BuildNode(child, childrenByParent, visited, new HashSet<short>(path));
                if (childNode is not null)
                {
                    children.Add(childNode);
                }
            }
        }

        return new CategoryTreeDto(
            category.CategoryID,
            category.CategoryName,
            category.CategoryDesciption,
            category.IsActive,
            children);
    }
}
