using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace mvcIpsa.DbModel
{
    public partial class IPSAContext : DbContext
    {
        public virtual DbSet<Bancos> Bancos { get; set; }
        public virtual DbSet<BancosCuentas> BancosCuentas { get; set; }
        public virtual DbSet<Caja> Caja { get; set; }
        public virtual DbSet<CajaCuentaContable> CajaCuentaContable { get; set; }
        public virtual DbSet<CajaEstado> CajaEstado { get; set; }
        public virtual DbSet<CambioOficial> CambioOficial { get; set; }
        public virtual DbSet<Cliente> Cliente { get; set; }
        public virtual DbSet<Fondos> Fondos { get; set; }
        public virtual DbSet<IngresosEgresosCaja> IngresosEgresosCaja { get; set; }
        public virtual DbSet<IngresosEgresosCajaDetalle> IngresosEgresosCajaDetalle { get; set; }
        public virtual DbSet<LoteRecibos> LoteRecibos { get; set; }
        public virtual DbSet<MaestroContable> MaestroContable { get; set; }
        public virtual DbSet<Profile> Profile { get; set; }
        public virtual DbSet<Profilerole> Profilerole { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<TipoCliente> TipoCliente { get; set; }
        public virtual DbSet<TipoDocumento> TipoDocumento { get; set; }
        public virtual DbSet<TipoIngreso> TipoIngreso { get; set; }
        public virtual DbSet<TipoMoneda> TipoMoneda { get; set; }
        public virtual DbSet<TipoMovimiento> TipoMovimiento { get; set; }
        public virtual DbSet<TipoPago> TipoPago { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseNpgsql(@"Server=192.168.0.11;Port=5432;User Id=postgres;Password=123456*;Database=IPSA;");
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
                    .HasColumnType("char(20)")
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

            modelBuilder.Entity<Caja>(entity =>
            {
                entity.ToTable("caja", "siscb_core");

                entity.HasIndex(e => e.NoCaja)
                    .HasName("IX_noCaja")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('siscb_core.\"Caja_id_seq\"'::regclass)");

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.NoCaja).HasColumnName("noCaja");
            });

            modelBuilder.Entity<CajaCuentaContable>(entity =>
            {
                entity.ToTable("caja_cuenta_contable", "siscb_core");

                entity.HasIndex(e => e.CtaCuenta)
                    .HasName("IX_cta_cuenta");

                entity.HasIndex(e => e.IdCaja)
                    .HasName("IX_idCaja");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('siscb_core.cajacuentacontable_id_seq'::regclass)");

                entity.Property(e => e.CtaCuenta)
                    .IsRequired()
                    .HasColumnName("cta_cuenta");

                entity.Property(e => e.IdCaja).HasColumnName("idCaja");

                entity.HasOne(d => d.CtaCuentaNavigation)
                    .WithMany(p => p.CajaCuentaContable)
                    .HasForeignKey(d => d.CtaCuenta)
                    .HasConstraintName("FK_cta_cuenta");

                entity.HasOne(d => d.IdCajaNavigation)
                    .WithMany(p => p.CajaCuentaContable)
                    .HasForeignKey(d => d.IdCaja)
                    .HasConstraintName("FK_idCaja");
            });

            modelBuilder.Entity<CajaEstado>(entity =>
            {
                entity.HasKey(e => e.Nestado);

                entity.ToTable("caja_estado", "siscb_core");

                entity.Property(e => e.Nestado)
                    .HasColumnName("nestado")
                    .HasDefaultValueSql("nextval('siscb_core.ceja_estado_nestado_seq'::regclass)");

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasColumnName("descripcion")
                    .HasColumnType("varchar");
            });

            modelBuilder.Entity<CambioOficial>(entity =>
            {
                entity.HasKey(e => e.FechaCambioOficial);

                entity.ToTable("cambio_oficial");

                entity.Property(e => e.FechaCambioOficial)
                    .HasColumnName("fecha_cambio_oficial")
                    .HasDefaultValueSql("NULL::timestamp without time zone");

                entity.Property(e => e.Dolares).HasColumnName("dolares");

                entity.Property(e => e.Euros).HasColumnName("euros");

                entity.Property(e => e.Usuarioid)
                    .HasColumnName("usuarioid")
                    .HasColumnType("char(5)")
                    .HasDefaultValueSql("NULL::bpchar");
            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.Identificacion);

                entity.ToTable("cliente");

                entity.Property(e => e.Identificacion)
                    .HasColumnName("identificacion")
                    .ValueGeneratedNever();

                entity.Property(e => e.Apellido).HasColumnName("apellido");

                entity.Property(e => e.Correo).HasColumnName("correo");

                entity.Property(e => e.Direccion).HasColumnName("direccion");

                entity.Property(e => e.Idtipocliente).HasColumnName("idtipocliente");

                entity.Property(e => e.Nombre).HasColumnName("nombre");

                entity.Property(e => e.Numeroruc).HasColumnName("numeroruc");

                entity.Property(e => e.Telefono).HasColumnName("telefono");

                entity.HasOne(d => d.IdtipoclienteNavigation)
                    .WithMany(p => p.Cliente)
                    .HasForeignKey(d => d.Idtipocliente)
                    .HasConstraintName("PK_idtipocliente_c");
            });

            modelBuilder.Entity<Fondos>(entity =>
            {
                entity.HasKey(e => e.Fondoid);

                entity.ToTable("fondos");

                entity.Property(e => e.Fondoid)
                    .HasColumnName("fondoid")
                    .ValueGeneratedNever();

                entity.Property(e => e.Cuentaid)
                    .HasColumnName("cuentaid")
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.Cuentaidos)
                    .HasColumnName("cuentaidos")
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.FechaInserta)
                    .HasColumnName("fecha_inserta")
                    .HasColumnType("timestamptz")
                    .HasDefaultValueSql("NULL::timestamp with time zone");

                entity.Property(e => e.FechaModifica)
                    .HasColumnName("fecha_modifica")
                    .HasDefaultValueSql("NULL::timestamp without time zone");

                entity.Property(e => e.Nombre)
                    .HasColumnName("nombre")
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.UserInserta)
                    .HasColumnName("user_inserta")
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.UserModifica)
                    .HasColumnName("user_modifica")
                    .HasDefaultValueSql("NULL::character varying");
            });

            modelBuilder.Entity<IngresosEgresosCaja>(entity =>
            {
                entity.HasKey(e => e.Idrecibo);

                entity.ToTable("ingresos_egresos_caja", "siscb_core");

                entity.HasIndex(e => e.IdCaja)
                    .HasName("Ix?IdCaja");

                entity.HasIndex(e => e.Numrecibo)
                    .HasName("Ix_NumRecibo");

                entity.Property(e => e.Idrecibo)
                    .HasColumnName("idrecibo")
                    .HasDefaultValueSql("nextval('siscb_core.ingresos_egresos_caja_id_seq'::regclass)");

                entity.Property(e => e.Concepto)
                    .HasColumnName("concepto")
                    .HasColumnType("varchar");

                entity.Property(e => e.Cuentabanco)
                    .HasColumnName("cuentabanco")
                    .HasColumnType("varchar");

                entity.Property(e => e.Cuentacontablebanco)
                    .HasColumnName("cuentacontablebanco")
                    .HasColumnType("varchar");

                entity.Property(e => e.FechaProceso)
                    .HasColumnName("fecha_proceso")
                    .HasColumnType("date");

                entity.Property(e => e.Fecharegistro)
                    .HasColumnName("fecharegistro")
                    .HasColumnType("date");

                entity.Property(e => e.IdCaja).HasColumnName("idCaja");

                entity.Property(e => e.Identificacioncliente)
                    .HasColumnName("identificacioncliente")
                    .HasColumnType("varchar");

                entity.Property(e => e.Idtipoingreso).HasColumnName("idtipoingreso");

                entity.Property(e => e.Idtipomoneda).HasColumnName("idtipomoneda");

                entity.Property(e => e.Idtipopago).HasColumnName("idtipopago");

                entity.Property(e => e.Monto)
                    .HasColumnName("monto")
                    .HasColumnType("numeric(10, 2)");

                entity.Property(e => e.Montocheque)
                    .HasColumnName("montocheque")
                    .HasColumnType("numeric(10, 2)");

                entity.Property(e => e.Montoefectivo)
                    .HasColumnName("montoefectivo")
                    .HasColumnType("numeric(10, 2)");

                entity.Property(e => e.Montominuta)
                    .HasColumnName("montominuta")
                    .HasColumnType("numeric(10, 2)");

                entity.Property(e => e.Montotransferencia)
                    .HasColumnName("montotransferencia")
                    .HasColumnType("numeric(10, 2)");

                entity.Property(e => e.Ncentrocosto).HasColumnName("ncentrocosto");

                entity.Property(e => e.Nestado).HasColumnName("nestado");

                entity.Property(e => e.Noordenpago)
                    .HasColumnName("noordenpago")
                    .HasColumnType("varchar");

                entity.Property(e => e.Noreferencia)
                    .HasColumnName("noreferencia")
                    .HasColumnType("varchar");

                entity.Property(e => e.Numrecibo)
                    .IsRequired()
                    .HasColumnName("numrecibo")
                    .HasColumnType("varchar");

                entity.Property(e => e.Tipocambio)
                    .HasColumnName("tipocambio")
                    .HasColumnType("numeric(10, 2)");

                entity.Property(e => e.Tipomov).HasColumnName("tipomov");

                entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .HasColumnType("varchar");

                entity.HasOne(d => d.IdCajaNavigation)
                    .WithMany(p => p.IngresosEgresosCaja)
                    .HasForeignKey(d => d.IdCaja)
                    .HasConstraintName("PK_idCaja");

                entity.HasOne(d => d.IdtipoingresoNavigation)
                    .WithMany(p => p.IngresosEgresosCaja)
                    .HasForeignKey(d => d.Idtipoingreso)
                    .HasConstraintName("PK_idtipoingreso_iec");

                entity.HasOne(d => d.IdtipomonedaNavigation)
                    .WithMany(p => p.IngresosEgresosCaja)
                    .HasForeignKey(d => d.Idtipomoneda)
                    .HasConstraintName("PK_idtipomoneda_iec");

                entity.HasOne(d => d.IdtipopagoNavigation)
                    .WithMany(p => p.IngresosEgresosCaja)
                    .HasForeignKey(d => d.Idtipopago)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PK_idtipopago_iec");

                entity.HasOne(d => d.NestadoNavigation)
                    .WithMany(p => p.IngresosEgresosCaja)
                    .HasForeignKey(d => d.Nestado)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PK_estado_ice");

                entity.HasOne(d => d.TipomovNavigation)
                    .WithMany(p => p.IngresosEgresosCaja)
                    .HasForeignKey(d => d.Tipomov)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PK_tipomov_iec");
            });

            modelBuilder.Entity<IngresosEgresosCajaDetalle>(entity =>
            {
                entity.ToTable("ingresos_egresos_caja_detalle", "siscb_core");

                entity.HasIndex(e => e.Idrecibo)
                    .HasName("IX_idrecibo");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('siscb_core.ingresos_egresos_caja_detalle_id_seq'::regclass)");

                entity.Property(e => e.Cantidad).HasColumnName("cantidad");

                entity.Property(e => e.CtaContable)
                    .IsRequired()
                    .HasColumnName("cta_contable");

                entity.Property(e => e.Idrecibo).HasColumnName("idrecibo");

                entity.Property(e => e.Montocordoba)
                    .HasColumnName("montocordoba")
                    .HasColumnType("numeric(32, 2)");

                entity.Property(e => e.Montodolar)
                    .HasColumnName("montodolar")
                    .HasColumnType("numeric(32, 2)");

                entity.Property(e => e.Precio)
                    .HasColumnName("precio")
                    .HasColumnType("numeric(32, 2)");

                entity.HasOne(d => d.CtaContableNavigation)
                    .WithMany(p => p.IngresosEgresosCajaDetalle)
                    .HasForeignKey(d => d.CtaContable)
                    .HasConstraintName("FK_ctacontable");

                entity.HasOne(d => d.IdreciboNavigation)
                    .WithMany(p => p.IngresosEgresosCajaDetalle)
                    .HasForeignKey(d => d.Idrecibo)
                    .HasConstraintName("FK_idrecibo");
            });

            modelBuilder.Entity<LoteRecibos>(entity =>
            {
                entity.ToTable("lote_recibos", "siscb_core");

                entity.HasIndex(e => e.IdCaja)
                    .HasName("IX_IdCaja")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('siscb_core.\"loterecibos_Id_seq\"'::regclass)");
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

            modelBuilder.Entity<Profile>(entity =>
            {
                entity.HasKey(e => e.Username);

                entity.ToTable("profile", "admin_core");

                entity.HasIndex(e => e.Username)
                    .HasName("username_index_profile");

                entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .ValueGeneratedNever();

                entity.Property(e => e.Apellido)
                    .IsRequired()
                    .HasColumnName("apellido");

                entity.Property(e => e.Correo)
                    .IsRequired()
                    .HasColumnName("correo");

                entity.Property(e => e.Idcaja).HasColumnName("idcaja");

                entity.Property(e => e.Ncentrocosto).HasColumnName("ncentrocosto");

                entity.Property(e => e.Nestado).HasColumnName("nestado");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnName("nombre");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password");

                entity.HasOne(d => d.IdcajaNavigation)
                    .WithMany(p => p.Profile)
                    .HasForeignKey(d => d.Idcaja)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Caja");
            });

            modelBuilder.Entity<Profilerole>(entity =>
            {
                entity.HasKey(e => new { e.Username, e.Idrole });

                entity.ToTable("profilerole", "admin_core");

                entity.Property(e => e.Username).HasColumnName("username");

                entity.Property(e => e.Idrole).HasColumnName("idrole");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role", "admin_core");

                entity.HasIndex(e => e.Id)
                    .HasName("IdRole")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('admin_core.user_id_seq'::regclass)");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.Nestado).HasColumnName("nestado");
            });

            modelBuilder.Entity<TipoCliente>(entity =>
            {
                entity.HasKey(e => e.Idtipocliente);

                entity.ToTable("tipo_cliente");

                entity.Property(e => e.Idtipocliente).HasColumnName("idtipocliente");

                entity.Property(e => e.Tipocliente)
                    .HasColumnName("tipocliente")
                    .HasColumnType("varchar");
            });

            modelBuilder.Entity<TipoDocumento>(entity =>
            {
                entity.HasKey(e => e.TipoDoc);

                entity.ToTable("tipo_documento", "siscb_core");

                entity.Property(e => e.TipoDoc)
                    .HasColumnName("tipo_doc")
                    .HasDefaultValueSql("nextval('siscb_core.tipo_documento_tipo_doc_seq'::regclass)");

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasColumnName("descripcion")
                    .HasColumnType("varchar");
            });

            modelBuilder.Entity<TipoIngreso>(entity =>
            {
                entity.HasKey(e => e.Idtipoingreso);

                entity.ToTable("tipo_ingreso", "siscb_core");

                entity.Property(e => e.Idtipoingreso)
                    .HasColumnName("idtipoingreso")
                    .HasDefaultValueSql("nextval('siscb_core.tipoingreso_idtipoingreso_seq'::regclass)");

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasColumnName("descripcion")
                    .HasColumnType("varchar");
            });

            modelBuilder.Entity<TipoMoneda>(entity =>
            {
                entity.HasKey(e => e.Idtipomoneda);

                entity.ToTable("tipo_moneda", "siscb_core");

                entity.Property(e => e.Idtipomoneda)
                    .HasColumnName("idtipomoneda")
                    .HasDefaultValueSql("nextval('siscb_core.tipomoneda_idtipomoneda_seq'::regclass)");

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasColumnName("descripcion")
                    .HasColumnType("varchar");
            });

            modelBuilder.Entity<TipoMovimiento>(entity =>
            {
                entity.HasKey(e => e.Idtipomovimiento);

                entity.ToTable("tipo_movimiento", "siscb_core");

                entity.Property(e => e.Idtipomovimiento)
                    .HasColumnName("idtipomovimiento")
                    .HasDefaultValueSql("nextval('siscb_core.tipo_movimiento_idtipomovimiento_seq'::regclass)");

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasColumnName("descripcion")
                    .HasColumnType("varchar");

                entity.Property(e => e.TipoDoc).HasColumnName("tipo_doc");
            });

            modelBuilder.Entity<TipoPago>(entity =>
            {
                entity.HasKey(e => e.Idtipopago);

                entity.ToTable("tipo_pago", "siscb_core");

                entity.Property(e => e.Idtipopago)
                    .HasColumnName("idtipopago")
                    .HasDefaultValueSql("nextval('siscb_core.tipopago_id_seq'::regclass)");

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasColumnName("descripcion")
                    .HasColumnType("varchar");
            });

            modelBuilder.HasSequence("user_id_seq");

            modelBuilder.HasSequence("Caja_id_seq");

            modelBuilder.HasSequence("cajacuentacontable_id_seq");

            modelBuilder.HasSequence("ceja_estado_nestado_seq");

            modelBuilder.HasSequence("ingresos_egresos_caja_detalle_id_seq");

            modelBuilder.HasSequence("ingresos_egresos_caja_id_seq");

            modelBuilder.HasSequence("loterecibos_Id_seq");

            modelBuilder.HasSequence("tipo_documento_tipo_doc_seq");

            modelBuilder.HasSequence("tipo_movimiento_idtipomovimiento_seq");

            modelBuilder.HasSequence("tipoingreso_idtipoingreso_seq");

            modelBuilder.HasSequence("tipomoneda_idtipomoneda_seq");

            modelBuilder.HasSequence("tipopago_id_seq");
        }
    }
}
