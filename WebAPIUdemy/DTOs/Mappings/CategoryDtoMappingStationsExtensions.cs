﻿using WebAPIUdemy.Model;

namespace WebAPIUdemy.DTOs.Mappings;

public static class CategoryDtoMappingStationsExtensions
{
    public static CategoryDTO? ToCategoryDTO(this Category category)
    {
        if (category is null)
            return null;

        return new CategoryDTO
        {
            CategoryId = category.CategoryId,
            Name = category.Name!,
            ImageUrl = category.ImageUrl!
        };
    }

    public static Category? ToCartegory(this CategoryDTO categoryDto)
    {
        if (categoryDto is null) return null;

        return new Category
        {
            CategoryId = categoryDto.CategoryId,
            Name = categoryDto.Name!,
            ImageUrl = categoryDto.ImageUrl!
        };
    }


    public static IEnumerable<CategoryDTO> ToCategoryDtoList(this IEnumerable<Category> categories)
    {
        if (categories is null || !categories.Any())
        {
            return new List<CategoryDTO>();
        }

        return categories.Select(category => new CategoryDTO
        {
            CategoryId = category.CategoryId,
            Name = category.Name!,
            ImageUrl = category.ImageUrl!
        }).ToList();
    }

}

