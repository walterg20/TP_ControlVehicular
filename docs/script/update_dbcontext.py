import io

with io.open('Datos/Data/CVDbContext.cs', 'r', encoding='utf-8') as f:
    content = f.read()

content = content.replace(
    'public DbSet<Rol> Roles { get; set; } = null!;',
    'public DbSet<Rol> Roles { get; set; } = null!;\n        public DbSet<Permiso> Permisos { get; set; } = null!;'
)

config_str = '''
            modelBuilder.Entity<Permiso>().ToTable("Permisos").HasKey(p => p.IdPermiso);
            modelBuilder.Entity<Rol>()
                .HasMany(r => r.Permisos)
                .WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RolPermisos",
                    j => j.HasOne<Permiso>().WithMany().HasForeignKey("IdPermiso"),
                    j => j.HasOne<Rol>().WithMany().HasForeignKey("IdRol")
                );

            base.OnModelCreating(modelBuilder);'''

content = content.replace('base.OnModelCreating(modelBuilder);', config_str)

with io.open('Datos/Data/CVDbContext.cs', 'w', encoding='utf-8') as f:
    f.write(content)