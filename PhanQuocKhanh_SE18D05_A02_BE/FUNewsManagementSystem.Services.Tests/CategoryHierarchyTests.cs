using FUNewsManagementSystem.BusinessObjects.Entities;
using FUNewsManagementSystem.Services.Helpers;

namespace FUNewsManagementSystem.Services.Tests;

public sealed class CategoryHierarchyTests
{
    [Fact]
    public void WouldCreateCycle_detects_indirect_parent_cycle()
    {
        var categories = new List<Category>
        {
            Category(1, null),
            Category(2, 1),
            Category(3, 2)
        };

        Assert.True(CategoryHierarchy.WouldCreateCycle(categories, categoryId: 1, proposedParentId: 3));
        Assert.False(CategoryHierarchy.WouldCreateCycle(categories, categoryId: 3, proposedParentId: 1));
    }

    [Fact]
    public void BuildTree_preserves_three_levels()
    {
        var categories = new List<Category>
        {
            Category(1, null, "Campus"),
            Category(2, 1, "Students"),
            Category(3, 2, "Clubs")
        };

        var tree = CategoryHierarchy.BuildTree(categories);

        Assert.Equal("Campus", Assert.Single(tree).CategoryName);
        Assert.Equal("Students", Assert.Single(tree[0].Children).CategoryName);
        Assert.Equal("Clubs", Assert.Single(tree[0].Children[0].Children).CategoryName);
    }

    private static Category Category(short id, short? parentId, string? name = null) => new()
    {
        CategoryID = id,
        ParentCategoryID = parentId,
        CategoryName = name ?? $"Category {id}",
        CategoryDesciption = "Description",
        IsActive = true
    };
}
