﻿using System.Text.Json;

namespace DesignPatterns.Services;

public class DesignPatternsService : ISingletonDependency
{
    private List<Category> Categories = new();
    private List<Pattern> Patterns = new();

    public DesignPatternsService()
    {
        Task.Run(async () =>
        {
            await LoadCategoriesFromJson();
            await LoadPatternsFromFile();
        });
    }

    private async Task LoadCategoriesFromJson()
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("categories.json");
        using var streamReader = new StreamReader(stream);
        var content = streamReader.ReadToEnd();
        Categories = JsonSerializer.Deserialize<List<Category>>(content,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
    }

    private async Task LoadPatternsFromFile()
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("patterns.json");
        using var streamReader = new StreamReader(stream);
        var content = streamReader.ReadToEnd();
        Patterns = JsonSerializer.Deserialize<List<Pattern>>(content,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        await LoadIfEmpty();
        return Categories;
    }

    public async Task<Category> GetCategoryAsync(int categoryId)
    {
        await LoadIfEmpty();
        return Categories.FirstOrDefault(x => x.Id == categoryId);
    }

    private async Task LoadIfEmpty()
    {
        if (Categories.Count == 0)
            await LoadCategoriesFromJson();

        if (Patterns.Count == 0)
            await LoadPatternsFromFile();
    }

    public async Task<List<Pattern>> GetCategoryPatternsAsync(int categoryId)
    {
        await LoadIfEmpty();
        return Patterns.Where(x => x.CategoryId == categoryId).ToList();
    }
}