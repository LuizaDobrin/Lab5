﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TaskAgendaProj.Models;

namespace TaskAgendaProj.Migrations
{
    [DbContext(typeof(TasksDbContext))]
    [Migration("20190530105212_MakeUsernameUnique")]
    partial class MakeUsernameUnique
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TaskAgendaProj.Models.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Important");

                    b.Property<string>("Text");

                    b.Property<int?>("taskId");

                    b.HasKey("Id");

                    b.HasIndex("taskId");

                    b.ToTable("Comment");
                });

            modelBuilder.Entity("TaskAgendaProj.Models.Task", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateTimeAdded");

                    b.Property<DateTime?>("DateTimeClosedAt");

                    b.Property<DateTime>("Deadline");

                    b.Property<string>("Description");

                    b.Property<string>("Importance");

                    b.Property<string>("Status");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("TaskAgendaProj.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<string>("Password");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.HasIndex("Username")
                        .IsUnique()
                        .HasFilter("[Username] IS NOT NULL");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TaskAgendaProj.Models.Comment", b =>
                {
                    b.HasOne("TaskAgendaProj.Models.Task", "task")
                        .WithMany("Comments")
                        .HasForeignKey("taskId");
                });
#pragma warning restore 612, 618
        }
    }
}