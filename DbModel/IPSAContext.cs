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
        public virtual DbSet<IngresosEgresosBanco> IngresosEgresosBanco { get; set; }
        public virtual DbSet<IngresosEgresosBancoEstado> IngresosEgresosBancoEstado { get; set; }
        public virtual DbSet<IngresosEgresosCaja> IngresosEgresosCaja { get; set; }
        public virtual DbSet<IngresosEgresosCajaDetalle> IngresosEgresosCajaDetalle { get; set; }
        public virtual DbSet<IngresosEgresosCajaReferencias> IngresosEgresosCajaReferencias { get; set; }
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
                optionsBuilder.UseNpgsql(@"Server=192.168.0.21;Port=5432;User Id=postgres;Password=123456*;Database=IPSA;");
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

                entity.Property(e => e.NoCaja).HasColumnName("no_caja");
            });

            modelBuilder.Entity<CajaCuentaContable>(entity =>
            {
                entity.ToTable("caja_cuenta_contable", "siscb_core");

                entity.HasIndex(e => e.CajaId)
                    .HasName("IX_caja_id");

                entity.HasIndex(e => e.CtaCuenta)
                    .HasName("IX_cta_cuenta");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('siscb_core.cajacuentacontable_id_seq'::regclass)");

                entity.Property(e => e.CajaId).HasColumnName("caja_id");

                entity.Property(e => e.CtaCuenta)
                    .IsRequired()
                    .HasColumnName("cta_cuenta")
                    .HasDefaultValueSql("NULL::character varying");

                entity.HasOne(d => d.Caja)
                    .WithMany(p => p.CajaCuentaContable)
                    .HasForeignKey(d => d.CajaId)
                    .HasConstraintName("FK_idCaja");
            });

            modelBuilder.Entity<CajaEstado>(entity =>
            {
                entity.ToTable("caja_estado", "siscb_core");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
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
                entity.ToTable("cliente");

                entity.HasIndex(e => e.Identificacion)
                    .HasName("IX_identificacion")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Apellido)
                    .IsRequired()
                    .HasColumnName("apellido")
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.Correo)
                    .HasColumnName("correo")
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.Direccion)
                    .HasColumnName("direccion")
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.Identificacion)
                    .IsRequired()
                    .HasColumnName("identificacion")
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnName("nombre")
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.Telefono)
                    .HasColumnName("telefono")
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.TipoClienteId).HasColumnName("tipo_cliente_id");

                entity.HasOne(d => d.TipoCliente)
                    .WithMany(p => p.Cliente)
                    .HasForeignKey(d => d.TipoClienteId)
                    .HasConstraintName("FK_tipo_cliente_id_c");
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

            modelBuilder.Entity<IngresosEgresosBanco>(entity =>
            {
                entity.ToTable("ingresos_egresos_banco", "siscb_core");

                entity.HasIndex(e => e.CajaId)
                    .HasName("Ix_ieb_caja_id");

                entity.HasIndex(e => e.FechaProceso)
                    .HasName("Ix_ieb_fecha_proceso");

                entity.HasIndex(e => e.TipoMonedaId)
                    .HasName("Ix_ieb_tipo_moneda_id");

                entity.HasIndex(e => e.Username)
                    .HasName("Ix_ieb_username");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('siscb_core.ingresos_egresos_banco_id_seq'::regclass)");

                entity.Property(e => e.CajaId).HasColumnName("caja_id");

                entity.Property(e => e.Concepto)
                    .IsRequired()
                    .HasColumnName("concepto");

                entity.Property(e => e.CuentaContableBanco)
                    .IsRequired()
                    .HasColumnName("cuenta_contable_banco");

                entity.Property(e => e.EstadoId).HasColumnName("estado_id");

                entity.Property(e => e.FechaProceso).HasColumnName("fecha_proceso");

                entity.Property(e => e.FechaRegistro).HasColumnName("fecha_registro");

                entity.Property(e => e.Monto)
                    .HasColumnName("monto")
                    .HasColumnType("numeric(10, 4)");

                entity.Property(e => e.MotivoAnulado)
                    .HasColumnName("motivo_anulado")
                    .HasColumnType("varchar");

                entity.Property(e => e.Referencia)
                    .IsRequired()
                    .HasColumnName("referencia");

                entity.Property(e => e.TipoCambio)
                    .HasColumnName("tipo_cambio")
                    .HasColumnType("numeric(10, 4)");

                entity.Property(e => e.TipoMonedaId).HasColumnName("tipo_moneda_id");

                entity.Property(e => e.TipoMovimientoId).HasColumnName("tipo_movimiento_id");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username");

                entity.HasOne(d => d.Caja)
                    .WithMany(p => p.IngresosEgresosBanco)
                    .HasForeignKey(d => d.CajaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PK_caja_id");

                entity.HasOne(d => d.Estado)
                    .WithMany(p => p.IngresosEgresosBanco)
                    .HasForeignKey(d => d.EstadoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PK_estado_id");

                entity.HasOne(d => d.TipoMoneda)
                    .WithMany(p => p.IngresosEgresosBanco)
                    .HasForeignKey(d => d.TipoMonedaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PK_tipo_moneda_id");

                entity.HasOne(d => d.TipoMovimiento)
                    .WithMany(p => p.IngresosEgresosBanco)
                    .HasForeignKey(d => d.TipoMovimientoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PK_tipo_movimiento_id");

                entity.HasOne(d => d.UsernameNavigation)
                    .WithMany(p => p.IngresosEgresosBanco)
                    .HasForeignKey(d => d.Username)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PK_username");
            });

            modelBuilder.Entity<IngresosEgresosBancoEstado>(entity =>
            {
                entity.ToTable("ingresos_egresos_banco_estado", "siscb_core");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('siscb_core.ingresos_egresos_banco_estado_id_seq'::regclass)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description");
            });

            modelBuilder.Entity<IngresosEgresosCaja>(entity =>
            {
                entity.ToTable("ingresos_egresos_caja", "siscb_core");

                entity.HasIndex(e => e.CajaId)
                    .HasName("Ix_caja_id");

                entity.HasIndex(e => e.FechaProceso)
                    .HasName("Ix_fecha_proceso");

                entity.HasIndex(e => e.NumRecibo)
                    .HasName("Ix_num_recibo");

                entity.HasIndex(e => e.TipoIngresoId)
                    .HasName("Ix_tipo_ingreso_id");

                entity.HasIndex(e => e.Username)
                    .HasName("Ix_username");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('siscb_core.ingresos_egresos_caja_id_seq'::regclass)");

                entity.Property(e => e.Beneficiario).HasColumnName("beneficiario");

                entity.Property(e => e.CajaId).HasColumnName("caja_id");

                entity.Property(e => e.ClienteId).HasColumnName("cliente_id");

                entity.Property(e => e.Concepto)
                    .HasColumnName("concepto")
                    .HasColumnType("varchar");

                entity.Property(e => e.EstadoId).HasColumnName("estado_id");

                entity.Property(e => e.FechaProceso)
                    .HasColumnName("fecha_proceso")
                    .HasColumnType("date");

                entity.Property(e => e.FechaRegistro)
                    .HasColumnName("fecha_registro")
                    .HasColumnType("date");

                entity.Property(e => e.MotivoAnulado).HasColumnName("motivo_anulado");

                entity.Property(e => e.Muestra).HasColumnName("muestra");

                entity.Property(e => e.NoOrdenPago)
                    .HasColumnName("no_orden_pago")
                    .HasColumnType("varchar");

                entity.Property(e => e.NumRecibo)
                    .IsRequired()
                    .HasColumnName("num_recibo")
                    .HasColumnType("varchar");

                entity.Property(e => e.Referencias).HasColumnName("referencias");

                entity.Property(e => e.TipoCleinteId).HasColumnName("tipo_cleinte_id");

                entity.Property(e => e.TipoIngresoId).HasColumnName("tipo_ingreso_id");

                entity.Property(e => e.TipoMonedaId).HasColumnName("tipo_moneda_id");

                entity.Property(e => e.TipoMovimientoId).HasColumnName("tipo_movimiento_id");

                entity.Property(e => e.Total)
                    .HasColumnName("total")
                    .HasColumnType("numeric(10, 4)");

                entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .HasColumnType("varchar");

                entity.HasOne(d => d.Caja)
                    .WithMany(p => p.IngresosEgresosCaja)
                    .HasForeignKey(d => d.CajaId)
                    .HasConstraintName("PK_id_caja");

                entity.HasOne(d => d.Estado)
                    .WithMany(p => p.IngresosEgresosCaja)
                    .HasForeignKey(d => d.EstadoId)
                    .HasConstraintName("PK_estado_ice");

                entity.HasOne(d => d.TipoIngreso)
                    .WithMany(p => p.IngresosEgresosCaja)
                    .HasForeignKey(d => d.TipoIngresoId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("PK_id_tipo_ingreso_iec");

                entity.HasOne(d => d.TipoMoneda)
                    .WithMany(p => p.IngresosEgresosCaja)
                    .HasForeignKey(d => d.TipoMonedaId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("PK_id_tipo_moneda_iec");

                entity.HasOne(d => d.TipoMovimiento)
                    .WithMany(p => p.IngresosEgresosCaja)
                    .HasForeignKey(d => d.TipoMovimientoId)
                    .HasConstraintName("PK_tipo_mov_iec");

                entity.HasOne(d => d.UsernameNavigation)
                    .WithMany(p => p.IngresosEgresosCaja)
                    .HasForeignKey(d => d.Username)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("PK_username");
            });

            modelBuilder.Entity<IngresosEgresosCajaDetalle>(entity =>
            {
                entity.ToTable("ingresos_egresos_caja_detalle", "siscb_core");

                entity.HasIndex(e => e.ReciboId)
                    .HasName("IX_idrecibo");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('siscb_core.ingresos_egresos_caja_detalle_id_seq'::regclass)");

                entity.Property(e => e.Cantidad).HasColumnName("cantidad");

                entity.Property(e => e.CtaContable)
                    .IsRequired()
                    .HasColumnName("cta_contable");

                entity.Property(e => e.Montodolar)
                    .HasColumnName("montodolar")
                    .HasColumnType("numeric(32, 4)");

                entity.Property(e => e.Precio)
                    .HasColumnName("precio")
                    .HasColumnType("numeric(32, 2)");

                entity.Property(e => e.ReciboId).HasColumnName("recibo_id");

                entity.HasOne(d => d.Recibo)
                    .WithMany(p => p.IngresosEgresosCajaDetalle)
                    .HasForeignKey(d => d.ReciboId)
                    .HasConstraintName("FK_idrecibo");
            });

            modelBuilder.Entity<IngresosEgresosCajaReferencias>(entity =>
            {
                entity.ToTable("ingresos_egresos_caja_referencias", "siscb_core");

                entity.HasIndex(e => e.ReciboId)
                    .HasName("IX_recibo_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('siscb_core.ingresos_egresos_caja_referencias_id_seq'::regclass)");

                entity.Property(e => e.Fecha)
                    .HasColumnName("fecha")
                    .HasColumnType("date");

                entity.Property(e => e.IdBanco).HasColumnName("id_banco");

                entity.Property(e => e.MontoCheq)
                    .HasColumnName("monto_cheq")
                    .HasColumnType("numeric(10, 4)");

                entity.Property(e => e.MontoEfectivo)
                    .HasColumnName("monto_efectivo")
                    .HasColumnType("numeric(10, 4)")
                    .HasDefaultValueSql("NULL::numeric");

                entity.Property(e => e.MontoMinu)
                    .HasColumnName("monto_minu")
                    .HasColumnType("numeric(10, 4)");

                entity.Property(e => e.MontoTrans)
                    .HasColumnName("monto_trans")
                    .HasColumnType("numeric(10, 4)");

                entity.Property(e => e.ReciboId).HasColumnName("recibo_id");

                entity.Property(e => e.Referencia).HasColumnName("referencia");

                entity.Property(e => e.TipoCambio)
                    .HasColumnName("tipo_cambio")
                    .HasColumnType("numeric(10, 4)");

                entity.Property(e => e.TipoPagoId).HasColumnName("tipo_pago_id");

                entity.Property(e => e.Total)
                    .HasColumnName("total")
                    .HasColumnType("numeric(10, 4)");

                entity.HasOne(d => d.Recibo)
                    .WithMany(p => p.IngresosEgresosCajaReferencias)
                    .HasForeignKey(d => d.ReciboId)
                    .HasConstraintName("FK_idrecibo");

                entity.HasOne(d => d.TipoPago)
                    .WithMany(p => p.IngresosEgresosCajaReferencias)
                    .HasForeignKey(d => d.TipoPagoId)
                    .HasConstraintName("FK_idtipopago");
            });

            modelBuilder.Entity<LoteRecibos>(entity =>
            {
                entity.ToTable("lote_recibos", "siscb_core");

                entity.HasIndex(e => e.CajaId)
                    .HasName("IX_caja_id_lote")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('siscb_core.\"loterecibos_Id_seq\"'::regclass)");

                entity.Property(e => e.CajaId).HasColumnName("caja_id");

                entity.HasOne(d => d.Caja)
                    .WithOne(p => p.LoteRecibos)
                    .HasForeignKey<LoteRecibos>(d => d.CajaId)
                    .HasConstraintName("FK_caja_id");
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

                entity.Property(e => e.CajaId).HasColumnName("caja_id");

                entity.Property(e => e.Correo)
                    .IsRequired()
                    .HasColumnName("correo");

                entity.Property(e => e.Ncentrocosto).HasColumnName("ncentrocosto");

                entity.Property(e => e.Nestado).HasColumnName("nestado");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnName("nombre");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password");

                entity.HasOne(d => d.Caja)
                    .WithMany(p => p.Profile)
                    .HasForeignKey(d => d.CajaId)
                    .HasConstraintName("FK_Caja");
            });

            modelBuilder.Entity<Profilerole>(entity =>
            {
                entity.HasKey(e => new { e.Username, e.RoleId });

                entity.ToTable("profilerole", "admin_core");

                entity.Property(e => e.Username).HasColumnName("username");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Profilerole)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_role_id");

                entity.HasOne(d => d.UsernameNavigation)
                    .WithMany(p => p.Profilerole)
                    .HasForeignKey(d => d.Username)
                    .HasConstraintName("FK_username");
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
                entity.ToTable("tipo_cliente");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('tipo_cliente_idtipocliente_seq'::regclass)");

                entity.Property(e => e.Tipocliente)
                    .HasColumnName("tipocliente")
                    .HasColumnType("varchar");
            });

            modelBuilder.Entity<TipoDocumento>(entity =>
            {
                entity.ToTable("tipo_documento", "siscb_core");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('siscb_core.tipo_documento_tipo_doc_seq'::regclass)");

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasColumnName("descripcion")
                    .HasColumnType("varchar");
            });

            modelBuilder.Entity<TipoIngreso>(entity =>
            {
                entity.ToTable("tipo_ingreso", "siscb_core");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('siscb_core.tipoingreso_idtipoingreso_seq'::regclass)");

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasColumnName("descripcion")
                    .HasColumnType("varchar");
            });

            modelBuilder.Entity<TipoMoneda>(entity =>
            {
                entity.ToTable("tipo_moneda", "siscb_core");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('siscb_core.tipomoneda_idtipomoneda_seq'::regclass)");

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasColumnName("descripcion")
                    .HasColumnType("varchar");
            });

            modelBuilder.Entity<TipoMovimiento>(entity =>
            {
                entity.ToTable("tipo_movimiento", "siscb_core");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('siscb_core.tipo_movimiento_idtipomovimiento_seq'::regclass)");

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasColumnName("descripcion")
                    .HasColumnType("varchar");

                entity.Property(e => e.TipoDoc).HasColumnName("tipo_doc");
            });

            modelBuilder.Entity<TipoPago>(entity =>
            {
                entity.ToTable("tipo_pago", "siscb_core");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('siscb_core.tipopago_id_seq'::regclass)");

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasColumnName("descripcion")
                    .HasColumnType("varchar");
            });

            modelBuilder.HasSequence("user_id_seq");

            modelBuilder.HasSequence("tipo_cliente_idtipocliente_seq");

            modelBuilder.HasSequence("Caja_id_seq");

            modelBuilder.HasSequence("cajacuentacontable_id_seq");

            modelBuilder.HasSequence("ceja_estado_nestado_seq");

            modelBuilder.HasSequence("ingresos_egresos_banco_estado_id_seq");

            modelBuilder.HasSequence("ingresos_egresos_banco_id_seq");

            modelBuilder.HasSequence("ingresos_egresos_caja_detalle_id_seq");

            modelBuilder.HasSequence("ingresos_egresos_caja_id_seq");

            modelBuilder.HasSequence("ingresos_egresos_caja_referencias_id_seq");

            modelBuilder.HasSequence("loterecibos_Id_seq");

            modelBuilder.HasSequence("tipo_documento_tipo_doc_seq");

            modelBuilder.HasSequence("tipo_movimiento_idtipomovimiento_seq");

            modelBuilder.HasSequence("tipoingreso_idtipoingreso_seq");

            modelBuilder.HasSequence("tipomoneda_idtipomoneda_seq");

            modelBuilder.HasSequence("tipopago_id_seq");
        }
    }
}
