﻿using System;
using System.Collections.Generic;
using System.Linq;
//using System.Threading.Tasks;
using TaskAgendaProj.Models;
using TaskAgendaProj.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskAgendaProj.ViewModels;
using Microsoft.AspNetCore.Authorization;
using TaskAgenda.ViewModels;

namespace TaskAgendaProj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private ITaskService taskService;
        private IUsersService usersService;
        public TasksController(ITaskService taskService, IUsersService usersService)
        {
            this.taskService = taskService;
            this.usersService = usersService;
        }
       
        /// <summary>
        /// Get all the tasks
        /// </summary>
        /// <returns>A list of tasks</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // GET: api/tasks
        [HttpGet]
        public PaginatedList<TaskGetModel> Get([FromQuery]DateTime? from, [FromQuery]DateTime? to, [FromQuery]int page = 1)
        {
            // TODO: make pagination work with /api/tasks/page/<page number>
            page = Math.Max(page, 1);
            return taskService.GetAll(page, from, to);
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // GET: api/tasks/2
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            var found = taskService.GetById(id);
            if (found == null)
            {
                return NotFound();
            }

            return Ok(found);
        }
        
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // POST: api/Expenses
        [Authorize]
        [HttpPost]
        public void Post([FromBody] TaskPostModel task)
        {
            User addedBy = usersService.GetCurrentUser(HttpContext);
            //if (addedBy.UserRole == UserRole.UserManager)
            //{
            //    return Forbid();
            //}
            taskService.Create(task, addedBy);
        }
       

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        // PUT: api/Expenses/2
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Task task)
        {

            var result = taskService.Upsert(id, task);
            return Ok(result);
        }

        // DELETE: api/ApiWithActions/2
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = taskService.Delete(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}