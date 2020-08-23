﻿using Store_MVC.Models.Data;
using Store_MVC.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Store_MVC.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop
        public ActionResult Categories()
        {
            // Объявляем модель типа List
            List<CategoryVM> categoryVMList;

            using (Db db = new Db())
            {
                // Инициализируем модель данными
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }

            // Возвращаем List в представление
            return View(categoryVMList);
        }

        // POST: Admin/Shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string categoryName)
        {
            // Строковая переменная ID 
            string id;

            using (Db db = new Db())
            {
                // Проверить на уникальность
                if (db.Categories.Any(x => x.Name == categoryName)) { return "titletaken"; }

                // Иницализировать модель DTO
                CategoryDTO dto = new CategoryDTO();

                // Заполняем данными модель
                dto.Name = categoryName;
                dto.ShortDesc = categoryName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;

                // Сохранить
                db.Categories.Add(dto);
                db.SaveChanges();

                // Получаем ID для возврата в представление 
                id = dto.Id.ToString();
            }

            // Вернуть ID в представление
            return id;
        }

        // Метод сортировки
        // POST: Admin/Shop/ReorderCategories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {

            using (Db db = new Db())
            {
                // Реализуем начальный счётчик
                int count = 0;

                // Инициализируем модель данных
                CategoryDTO dto;

                // Устанавливаем сортирвку для каждой страницы
                foreach (var categoryId in id)
                {
                    dto = db.Categories.Find(categoryId);
                    dto.Sorting = count;

                    db.SaveChanges();

                    count++;
                }
            }
        }

        // Метод удаления записей
        // GET: Admin/Pages/DeleteCategory/id
        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {
                // Получаем модель категории
                CategoryDTO dto = db.Categories.Find(id);

                // Удаляем категорию
                db.Categories.Remove(dto);

                // Сохраняем изменения в базе
                db.SaveChanges();
            }

            // Добавляем сообщение об успешном удалении
            TempData["SM"] = "You have deleted a category.";

            // Возвращаем пользователя
            return RedirectToAction("Categories");
        }

        // POST: Admin/Pages/DeleteCategory/id
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            using (Db db = new Db())
            {
                // Проверяем уникальность имени
                if (db.Categories.Any(x => x.Name == newCatName)) { return "titletaken"; }

                // Получаем модель DTO
                CategoryDTO dto = db.Categories.Find(id);

                // Редактируем модель DTO
                dto.Name = newCatName;
                dto.ShortDesc = newCatName.Replace(" ", "-").ToLower();

                // Сохраняем изменения
                db.SaveChanges();
            }

            // Возвращаем результат
            return "";
        }

        // Добавление товаров
        // GET: Admin/Shop/AddProduct
        public ActionResult AddProduct()
        {
            // Объявить модель данных
            ProductVM model = new ProductVM();

            // Добавляем список категорий
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

            // Возвращаем модель в представление
            return View(model);
        }
    }
}