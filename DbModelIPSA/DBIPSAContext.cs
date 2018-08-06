using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace mvcIpsa.DbModelIPSA
{
    public partial class DBIPSAContext : DbContext
    {
        public virtual DbSet<Bancos> Bancos { get; set; }
        public virtual DbSet<BancosCuentas> BancosCuentas { get; set; }
        public virtual DbSet<MaestroContable> MaestroContable { get; set; }

        // Unable to generate entity type for table 'public.areas'. Please see the warning messages.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseNpgsql(@"Server=192.168.0.11;Port=5432;User Id=postgres;Password=123456*;Database=DBIPSA;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bancos>(entity =>
            {
                entity.HasKey(e => e.Bancoid);

                entity.ToTable("bancos");

                entity.Property(e => e.Bancoid)
                    .HasColumnName("bancoid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasColumnName("descripcion")
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.Orden)
                    .HasColumnName("orden")
                    .ForNpgsqlHasComment("El orden en que aparecen los bancos en el combo bancos de recaudo");

                entity.Property(e => e.Siglas)
                    .HasColumnName("siglas")
                    .HasDefaultValueSql("NULL::character varying");
            });

            modelBuilder.Entity<BancosCuentas>(entity =>
            {
                entity.HasKey(e => new { e.BancoCuenta, e.Centroid });

                entity.ToTable("bancos_cuentas");

                entity.HasIndex(e => e.Moneda)
                    .HasName("bancos_cuentas_idx");

                entity.Property(e => e.BancoCuenta).HasColumnName("banco_cuenta");

                entity.Property(e => e.Centroid)
                    .HasColumnName("centroid")
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.Bancoid).HasColumnName("bancoid");

                entity.Property(e => e.CtaBancaria)
                    .HasColumnName("cta_bancaria")
                    .HasColumnType("char(10)")
                    .HasDefaultValueSql("NULL::bpchar");

                entity.Property(e => e.CtaContable)
                    .HasColumnName("cta_contable")
                    .HasColumnType("char(21)")
                    .HasDefaultValueSql("NULL::bpchar");

                entity.Property(e => e.Descripcion)
                    .HasColumnName("descripcion")
                    .HasColumnType("char(100)")
                    .HasDefaultValueSql("NULL::bpchar");

                entity.Property(e => e.EgresosTemp).HasColumnName("egresos_temp");

                entity.Property(e => e.Estado).HasColumnName("estado");

                entity.Property(e => e.IngresosTemp).HasColumnName("ingresos_temp");

                entity.Property(e => e.Moneda).HasColumnName("moneda");

                entity.Property(e => e.Multiminutas).HasColumnName("multiminutas");

                entity.Property(e => e.NombreSucursal)
                    .HasColumnName("nombre_sucursal")
                    .HasColumnType("char(50)")
                    .HasDefaultValueSql("NULL::bpchar");

                entity.Property(e => e.SaldoFinalTemp).HasColumnName("saldo_final_temp");

                entity.Property(e => e.SaldoInicialTemp).HasColumnName("saldo_inicial_temp");

                entity.HasOne(d => d.Banco)
                    .WithMany(p => p.BancosCuentas)
                    .HasForeignKey(d => d.Bancoid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("bancos_cuentas_bancoid_fkey");
            });

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

            modelBuilder.HasSequence("cliente_id_seq");

            modelBuilder.HasSequence("tipo_cliente_idtipocliente_seq");
        }
    }
}
