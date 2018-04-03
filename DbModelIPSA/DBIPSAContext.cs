using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace mvcIpsa.DbModelIPSA
{
    public partial class DBIPSAContext : DbContext
    {
        public virtual DbSet<MaestroContable> MaestroContable { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseNpgsql(@"Host=192.168.0.13;Port=5432;Database=DBIPSA;Username=postgres;Password=123456*");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MaestroContable>(entity =>
            {
                entity.HasKey(e => e.CtaContable);

                entity.ToTable("maestro_contable", "sconta");

                entity.HasIndex(e => e.Nombre)
                    .HasName("IX_nombre_mc");

                entity.Property(e => e.CtaContable)
                    .HasColumnName("cta_contable")
                    .HasDefaultValueSql("NULL::bpchar");

                entity.Property(e => e.Centro)
                    .HasColumnName("centro")
                    .HasColumnType("char(4)")
                    .HasDefaultValueSql("NULL::bpchar");

                entity.Property(e => e.CtaPadre)
                    .IsRequired()
                    .HasColumnName("cta_padre")
                    .HasDefaultValueSql("NULL::bpchar");

                entity.Property(e => e.Cuenta)
                    .IsRequired()
                    .HasColumnName("cuenta")
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.Mes1)
                    .HasColumnName("mes1")
                    .HasColumnType("numeric(19, 2)")
                    .HasDefaultValueSql("NULL::numeric");

                entity.Property(e => e.Mes10)
                    .HasColumnName("mes10")
                    .HasColumnType("numeric(19, 2)")
                    .HasDefaultValueSql("NULL::numeric");

                entity.Property(e => e.Mes11)
                    .HasColumnName("mes11")
                    .HasColumnType("numeric(19, 2)")
                    .HasDefaultValueSql("NULL::numeric");

                entity.Property(e => e.Mes12)
                    .HasColumnName("mes12")
                    .HasColumnType("numeric(19, 2)")
                    .HasDefaultValueSql("NULL::numeric");

                entity.Property(e => e.Mes2)
                    .HasColumnName("mes2")
                    .HasColumnType("numeric(19, 2)")
                    .HasDefaultValueSql("NULL::numeric");

                entity.Property(e => e.Mes3)
                    .HasColumnName("mes3")
                    .HasColumnType("numeric(19, 2)")
                    .HasDefaultValueSql("NULL::numeric");

                entity.Property(e => e.Mes4)
                    .HasColumnName("mes4")
                    .HasColumnType("numeric(19, 2)")
                    .HasDefaultValueSql("NULL::numeric");

                entity.Property(e => e.Mes5)
                    .HasColumnName("mes5")
                    .HasColumnType("numeric(19, 2)")
                    .HasDefaultValueSql("NULL::numeric");

                entity.Property(e => e.Mes6)
                    .HasColumnName("mes6")
                    .HasColumnType("numeric(19, 2)")
                    .HasDefaultValueSql("NULL::numeric");

                entity.Property(e => e.Mes7)
                    .HasColumnName("mes7")
                    .HasColumnType("numeric(19, 2)")
                    .HasDefaultValueSql("NULL::numeric");

                entity.Property(e => e.Mes8)
                    .HasColumnName("mes8")
                    .HasColumnType("numeric(19, 2)")
                    .HasDefaultValueSql("NULL::numeric");

                entity.Property(e => e.Mes9)
                    .HasColumnName("mes9")
                    .HasColumnType("numeric(19, 2)")
                    .HasDefaultValueSql("NULL::numeric");

                entity.Property(e => e.MovCreditos)
                    .HasColumnName("mov_creditos")
                    .HasColumnType("numeric(19, 2)")
                    .HasDefaultValueSql("NULL::numeric");

                entity.Property(e => e.MovCreditoshist)
                    .HasColumnName("mov_creditoshist")
                    .HasColumnType("numeric(19, 2)")
                    .HasDefaultValueSql("NULL::numeric");

                entity.Property(e => e.MovDebitos)
                    .HasColumnName("mov_debitos")
                    .HasColumnType("numeric(19, 2)")
                    .HasDefaultValueSql("NULL::numeric");

                entity.Property(e => e.MovDebitoshist)
                    .HasColumnName("mov_debitoshist")
                    .HasColumnType("numeric(19, 2)")
                    .HasDefaultValueSql("NULL::numeric");

                entity.Property(e => e.Movimiento).HasColumnName("movimiento");

                entity.Property(e => e.NivelCuenta).HasColumnName("nivel_cuenta");

                entity.Property(e => e.Nombre)
                    .HasColumnName("nombre")
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.SaldoFinal)
                    .HasColumnName("saldo_final")
                    .HasColumnType("numeric(19, 2)")
                    .HasDefaultValueSql("NULL::numeric");

                entity.Property(e => e.SaldoFinalhist)
                    .HasColumnName("saldo_finalhist")
                    .HasColumnType("numeric(19, 2)")
                    .HasDefaultValueSql("NULL::numeric");

                entity.Property(e => e.SaldoInicial)
                    .HasColumnName("saldo_inicial")
                    .HasColumnType("numeric(19, 2)")
                    .HasDefaultValueSql("NULL::numeric");

                entity.Property(e => e.SaldoInicialPeriodo)
                    .HasColumnName("saldo_inicial_periodo")
                    .HasColumnType("numeric(19, 2)")
                    .HasDefaultValueSql("NULL::numeric");

                entity.Property(e => e.SaldoInicialhist)
                    .HasColumnName("saldo_inicialhist")
                    .HasColumnType("numeric(19, 2)")
                    .HasDefaultValueSql("NULL::numeric");

                entity.Property(e => e.SaldoPreCierre)
                    .HasColumnName("saldo_pre_cierre")
                    .HasColumnType("numeric(19, 2)")
                    .HasDefaultValueSql("NULL::numeric");

                entity.Property(e => e.TipoCta).HasColumnName("tipo_cta");

                entity.Property(e => e.TipoDh).HasColumnName("tipo_dh");
            });
        }
    }
}
