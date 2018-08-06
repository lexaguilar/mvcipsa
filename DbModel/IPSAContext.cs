using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace mvcIpsa.DbModel
{
    public partial class IPSAContext : DbContext
    {       
        public virtual DbSet<Caja> Caja { get; set; }
        public virtual DbSet<CajaCuentaContable> CajaCuentaContable { get; set; }
        public virtual DbSet<CajaEstado> CajaEstado { get; set; }
        public virtual DbSet<CambioOficial> CambioOficial { get; set; }
        public virtual DbSet<Cliente> Cliente { get; set; }
        public virtual DbSet<ConciliacionBancaria> ConciliacionBancaria { get; set; }
        public virtual DbSet<ConciliacionBancariaAux> ConciliacionBancariaAux { get; set; }
        public virtual DbSet<ConciliacionBancariaEstado> ConciliacionBancariaEstado { get; set; }
        public virtual DbSet<CuentasBancoUsername> CuentasBancoUsername { get; set; }
        public virtual DbSet<IngresosEgresosBanco> IngresosEgresosBanco { get; set; }
        public virtual DbSet<IngresosEgresosBancoEstado> IngresosEgresosBancoEstado { get; set; }
        public virtual DbSet<IngresosEgresosCaja> IngresosEgresosCaja { get; set; }
        public virtual DbSet<IngresosEgresosCajaDetalle> IngresosEgresosCajaDetalle { get; set; }
        public virtual DbSet<IngresosEgresosCajaReferencias> IngresosEgresosCajaReferencias { get; set; }
        public virtual DbSet<LoteRecibos> LoteRecibos { get; set; }
        public virtual DbSet<ProcesoBanco> ProcesoBanco { get; set; }
        public virtual DbSet<Profile> Profile { get; set; }
        public virtual DbSet<Profilerole> Profilerole { get; set; }
        public virtual DbSet<ReporteFirma> ReporteFirma { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<TipoCliente> TipoCliente { get; set; }
        public virtual DbSet<TipoDocumento> TipoDocumento { get; set; }
        public virtual DbSet<TipoIngreso> TipoIngreso { get; set; }
        public virtual DbSet<TipoMoneda> TipoMoneda { get; set; }
        public virtual DbSet<TipoMovimiento> TipoMovimiento { get; set; }
        public virtual DbSet<TipoPago> TipoPago { get; set; }
        public virtual DbSet<TipoProceso> TipoProceso { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseNpgsql(@"Server=192.168.0.11;Port=5432;User Id=postgres;Password=123456*;Database=IPSACORE_COPY;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            modelBuilder.Entity<Caja>(entity =>
            {
                entity.ToTable("caja", "siscb_core");

                entity.HasIndex(e => e.NoCaja)
                    .HasName("IX_noCaja")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('siscb_core.\"Caja_id_seq\"'::regclass)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasDefaultValueSql("NULL::character varying");

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

            modelBuilder.Entity<ConciliacionBancaria>(entity =>
            {
                entity.ToTable("conciliacion_bancaria", "siscb_core");

                entity.HasIndex(e => e.ProcesoBancoId)
                    .HasName("Ix_proceso_banco_id");

                entity.HasIndex(e => e.TipoMovimientoId)
                    .HasName("Ix_tipo_movimiento_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('siscb_core.conciliacion_bancaria_iecb_id_seq'::regclass)");

                entity.Property(e => e.Credito)
                    .HasColumnName("credito")
                    .HasColumnType("numeric(10, 4)");

                entity.Property(e => e.Debito)
                    .HasColumnName("debito")
                    .HasColumnType("numeric(16, 4)");

                entity.Property(e => e.EstadoId).HasColumnName("estado_id");

                entity.Property(e => e.Fecha)
                    .HasColumnName("fecha")
                    .HasColumnType("date");

                entity.Property(e => e.ProcesoBancoId).HasColumnName("proceso_banco_id");

                entity.Property(e => e.Referencia)
                    .IsRequired()
                    .HasColumnName("referencia");

                entity.Property(e => e.TipoMovimientoId).HasColumnName("tipo_movimiento_id");

                entity.Property(e => e.Uuid)                  
                    .HasColumnName("uuid")
                    .HasColumnType("varchar");

                entity.Property(e => e.Conciliado).HasColumnName("conciliado").HasColumnType("bool");

                entity.HasOne(d => d.Estado)
                    .WithMany(p => p.ConciliacionBancaria)
                    .HasForeignKey(d => d.EstadoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PK_estado_id");

                entity.HasOne(d => d.ProcesoBanco)
                    .WithMany(p => p.ConciliacionBancaria)
                    .HasForeignKey(d => d.ProcesoBancoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PK_proceso_banco_id");

                entity.HasOne(d => d.TipoMovimiento)
                    .WithMany(p => p.ConciliacionBancaria)
                    .HasForeignKey(d => d.TipoMovimientoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PK_tipo_movimiento_id");
            });

            modelBuilder.Entity<ConciliacionBancariaAux>(entity =>
            {
                entity.ToTable("conciliacion_bancaria_aux", "siscb_core");

                entity.HasIndex(e => e.ProcesoBancoId)
                    .HasName("Ix_cba_proceso_banco_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('siscb_core.conciliacion_bancaria_id_seq'::regclass)");

                entity.Property(e => e.TableInfo)
                  .HasColumnName("table_info");

                entity.Property(e => e.IdOrigen)                    
                  .HasColumnName("id_origen");

                entity.HasIndex(e => e.IdOrigen).HasName("Ix_id_origen");              

                entity.Property(e => e.Credito)
                    .HasColumnName("credito")
                    .HasColumnType("numeric(10, 4)");

                entity.Property(e => e.Debito)
                    .HasColumnName("debito")
                    .HasColumnType("numeric(10, 4)");

                entity.Property(e => e.EstadoId).HasColumnName("estado_id");

                entity.Property(e => e.Fecha)
                    .HasColumnName("fecha")
                    .HasColumnType("date");

                entity.Property(e => e.ProcesoBancoId).HasColumnName("proceso_banco_id");

                entity.Property(e => e.Referencia)
                    .IsRequired()
                    .HasColumnName("referencia")
                    .HasColumnType("varchar");

                entity.Property(e => e.TipoMovimientoId).HasColumnName("tipo_movimiento_id");

                entity.Property(e => e.Uuid)                    
                    .HasColumnName("uuid")
                    .HasColumnType("varchar");

                entity.Property(e => e.Conciliado).HasColumnName("conciliado").HasColumnType("bool");

                entity.HasOne(d => d.Estado)
                    .WithMany(p => p.ConciliacionBancariaAux)
                    .HasForeignKey(d => d.EstadoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PK_estado_id");

                entity.HasOne(d => d.ProcesoBanco)
                    .WithMany(p => p.ConciliacionBancariaAux)
                    .HasForeignKey(d => d.ProcesoBancoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PK_proceso_banco_id");

                entity.HasOne(d => d.TipoMovimiento)
                    .WithMany(p => p.ConciliacionBancariaAux)
                    .HasForeignKey(d => d.TipoMovimientoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PK_tipo_movimiento_id");
            });

            modelBuilder.Entity<ConciliacionBancariaEstado>(entity =>
            {
                entity.ToTable("conciliacion_bancaria_estado", "siscb_core");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Descripcion).IsRequired();
            });

            modelBuilder.Entity<CuentasBancoUsername>(entity =>
            {
                entity.ToTable("cuentas_banco_username", "siscb_core");

                entity.HasIndex(e => e.Username)
                    .HasName("Ix_banco_cuenta_username");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('siscb_core.cuentas_banco_username_id_seq'::regclass)");

                entity.Property(e => e.BancoCuenta).HasColumnName("banco_cuenta");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasColumnType("varchar");

                entity.HasOne(d => d.UsernameNavigation)
                    .WithMany(p => p.CuentasBancoUsername)
                    .HasForeignKey(d => d.Username)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PK_username");
            });

            modelBuilder.Entity<IngresosEgresosBanco>(entity =>
            {
                entity.ToTable("ingresos_egresos_banco", "siscb_core");

                entity.HasIndex(e => e.BancoCuenta)
                    .HasName("Ix_ieb_cuenta_banco");

                entity.HasIndex(e => e.CajaId)
                    .HasName("Ix_ieb_caja_id");

                entity.HasIndex(e => e.FechaProceso)
                    .HasName("Ix_ieb_fecha_proceso");

                entity.HasIndex(e => e.TipoDocumentoId)
                    .HasName("Ixieb_tipo_documento_id");

                entity.HasIndex(e => e.TipoMonedaId)
                    .HasName("Ix_ieb_tipo_moneda_id");

                entity.HasIndex(e => e.Username)
                    .HasName("Ix_ieb_username");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('siscb_core.ingresos_egresos_banco_id_seq'::regclass)");

                entity.Property(e => e.BancoCuenta).HasColumnName("banco_cuenta");

                entity.Property(e => e.Procesado).HasColumnName("procesado").HasColumnType("bool");

                entity.Property(e => e.CajaId).HasColumnName("caja_id");

                entity.Property(e => e.Concepto)
                    .IsRequired()
                    .HasColumnName("concepto");

                entity.Property(e => e.EstadoId).HasColumnName("estado_id");

                entity.Property(e => e.FechaAnulado).HasColumnName("fecha_anulado");

                entity.Property(e => e.FechaEditado).HasColumnName("fecha_editado");

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

                entity.Property(e => e.TipoDocumentoId).HasColumnName("tipo_documento_id");

                entity.Property(e => e.TipoMonedaId).HasColumnName("tipo_moneda_id");

                entity.Property(e => e.TipoMovimientoId).HasColumnName("tipo_movimiento_id");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username");

                entity.Property(e => e.UsernameAnulado).HasColumnName("username_anulado");

                entity.Property(e => e.UsernameEditado).HasColumnName("username_editado");

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

                entity.HasOne(d => d.TipoDocumento)
                    .WithMany(p => p.IngresosEgresosBanco)
                    .HasForeignKey(d => d.TipoDocumentoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PK_tipo_documento_id");

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

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasColumnName("descripcion");
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

                entity.Property(e => e.Beneficiario)
                    .HasColumnName("beneficiario")
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.CajaId).HasColumnName("caja_id");

                entity.Property(e => e.ClienteId).HasColumnName("cliente_id");

                entity.Property(e => e.Concepto)
                    .HasColumnName("concepto")
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.EstadoId).HasColumnName("estado_id");

                entity.Property(e => e.FechaAnulado).HasColumnName("fecha_anulado");

                entity.Property(e => e.FechaEditado).HasColumnName("fecha_editado");

                entity.Property(e => e.FechaProceso)
                    .HasColumnName("fecha_proceso")
                    .HasColumnType("date");

                entity.Property(e => e.FechaRegistro).HasColumnName("fecha_registro");

                entity.Property(e => e.MotivoAnulado)
                    .HasColumnName("motivo_anulado")
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.Muestra)
                    .HasColumnName("muestra")
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.NoOrdenPago)
                    .HasColumnName("no_orden_pago")
                    .HasDefaultValueSql("NULL::character varying");

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
                    .HasColumnType("numeric(10, 4)")
                    .HasDefaultValueSql("NULL::numeric");

                entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.UsernameAnulado).HasColumnName("username_anulado");

                entity.Property(e => e.UsernameEditado).HasColumnName("username_editado");

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
                    .HasColumnName("cta_contable")
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.Montodolar)
                    .HasColumnName("montodolar")
                    .HasColumnType("numeric(32, 4)")
                    .HasDefaultValueSql("NULL::numeric");

                entity.Property(e => e.Precio)
                    .HasColumnName("precio")
                    .HasColumnType("numeric(32, 2)")
                    .HasDefaultValueSql("NULL::numeric");

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
                    .HasDefaultValueSql("nextval('siscb_core.ingresos_egresos_caja_referencias_id_seq1'::regclass)");

                entity.Property(e => e.Fecha)
                    .HasColumnName("fecha")
                    .HasColumnType("date");

                entity.Property(e => e.IdBanco).HasColumnName("id_banco");

                entity.Property(e => e.MontoCheq)
                    .HasColumnName("monto_cheq")
                    .HasColumnType("numeric(10, 4)");
                    //.HasDefaultValueSql("NULL::numeric");

                entity.Property(e => e.MontoEfectivo)
                    .HasColumnName("monto_efectivo")
                    .HasColumnType("numeric(10, 4)");
                    //.HasDefaultValueSql("NULL::numeric");

                entity.Property(e => e.MontoMinu)
                    .HasColumnName("monto_minu")
                    .HasColumnType("numeric(10, 4)");
                    //.HasDefaultValueSql("0::numeric");

                entity.Property(e => e.MontoTrans)
                    .HasColumnName("monto_trans")
                    .HasColumnType("numeric(10, 4)");
                    //.HasDefaultValueSql("0::numeric");

                entity.Property(e => e.ReciboId).HasColumnName("recibo_id");

                entity.Property(e => e.Referencia)
                    .HasColumnName("referencia")
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.TipoCambio)
                    .HasColumnName("tipo_cambio")
                    .HasColumnType("numeric(10, 4)")
                    .HasDefaultValueSql("NULL::numeric");

                entity.Property(e => e.TipoPagoId).HasColumnName("tipo_pago_id");
                entity.Property(e => e.Procesado).HasColumnName("procesado").HasColumnType("bool");
                entity.Property(e => e.Total)
                    .HasColumnName("total")
                    .HasColumnType("numeric(10, 4)")
                    .HasDefaultValueSql("NULL::numeric");

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
                    .HasName("IX_caja_id_lote");

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('siscb_core.\"loterecibos_Id_seq\"'::regclass)");

                entity.Property(e => e.CajaId).HasColumnName("caja_id");

                entity.HasOne(d => d.Caja)
                    .WithMany(p => p.LoteRecibos)
                    .HasForeignKey(d => d.CajaId)
                    .HasConstraintName("FK_caja_id");
            });

            modelBuilder.Entity<ProcesoBanco>(entity =>
            {
                entity.ToTable("proceso_banco", "siscb_core");

                entity.HasIndex(e => e.BancoCuenta)
                    .HasName("Ix_banco_cuenta");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('siscb_core.procesobanco_id_seq'::regclass)");

                entity.Property(e => e.BancoCuenta).HasColumnName("banco_cuenta");

                entity.Property(e => e.Fecha)
                    .HasColumnName("fecha")
                    .HasColumnType("date");

                entity.Property(e => e.FechaRegistrado).HasColumnName("fecha_registrado");

                entity.Property(e => e.SaldoFinal)
                    .HasColumnName("saldo_final")
                    .HasColumnType("numeric(16, 4)");

                entity.Property(e => e.SaldoInicial)
                    .HasColumnName("saldo_inicial")
                    .HasColumnType("numeric(16, 4)");

                entity.Property(e => e.TipoProcesoId).HasColumnName("tipo_proceso_id");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username");

                entity.HasOne(d => d.TipoProceso)
                    .WithMany(p => p.ProcesoBanco)
                    .HasForeignKey(d => d.TipoProcesoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PK_tipo_proceso_id");

                entity.HasOne(d => d.UsernameNavigation)
                    .WithMany(p => p.ProcesoBanco)
                    .HasForeignKey(d => d.Username)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PK_preceso_banco_username");
            });

            modelBuilder.Entity<Profile>(entity =>
            {
                entity.HasKey(e => e.Username);

                entity.ToTable("profile", "admin_core");

                entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.Apellido)
                    .IsRequired()
                    .HasColumnName("apellido")
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.CajaId).HasColumnName("caja_id");

                entity.Property(e => e.Correo)
                    .IsRequired()
                    .HasColumnName("correo")
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.Ncentrocosto).HasColumnName("ncentrocosto");

                entity.Property(e => e.Nestado).HasColumnName("nestado");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnName("nombre")
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasDefaultValueSql("NULL::character varying");

                entity.HasOne(d => d.Caja)
                    .WithMany(p => p.Profile)
                    .HasForeignKey(d => d.CajaId)
                    .HasConstraintName("FK_Caja");
            });

            modelBuilder.Entity<Profilerole>(entity =>
            {
                entity.HasKey(e => new { e.Username, e.RoleId });

                entity.ToTable("profilerole", "admin_core");

                entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .HasDefaultValueSql("NULL::character varying");

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

            modelBuilder.Entity<ReporteFirma>(entity =>
            {
                entity.HasKey(e => e.Reporte);

                entity.ToTable("reporte_firma", "siscb_core");

                entity.Property(e => e.Reporte)
                    .HasColumnName("reporte")
                    .ValueGeneratedNever();

                entity.Property(e => e.MostrarAprobado).HasColumnName("mostrar_aprobado");

                entity.Property(e => e.MostrarAutorizado).HasColumnName("mostrar_autorizado");

                entity.Property(e => e.MostrarElaborado).HasColumnName("mostrar_elaborado");

                entity.Property(e => e.MostrarRevisado).HasColumnName("mostrar_revisado");

                entity.Property(e => e.Ubicacion).HasColumnName("ubicacion");

                entity.Property(e => e.Nombre).HasColumnName("nombre");

                entity.Property(e => e.UsernameAprobado)
                    .IsRequired()
                    .HasColumnName("username_aprobado");

                entity.Property(e => e.UsernameAutorizado)
                    .IsRequired()
                    .HasColumnName("username_autorizado");

                entity.Property(e => e.UsernameElaborado)
                    .IsRequired()
                    .HasColumnName("username_elaborado");

                entity.Property(e => e.UsernameRevisado)
                    .IsRequired()
                    .HasColumnName("username_revisado");
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

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasDefaultValueSql("NULL::character varying");

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

                entity.Property(e => e.TipoDoc).HasColumnName("tipo_doc");
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

                entity.Property(e => e.DocName).HasColumnName("doc_name");

                entity.Property(e => e.TipoDoc).HasColumnName("tipo_doc");

                entity.HasOne(d => d.TipoDocNavigation)
                    .WithMany(p => p.TipoMovimiento)
                    .HasForeignKey(d => d.TipoDoc)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PK_tipo_documento_id");
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

            modelBuilder.Entity<TipoProceso>(entity =>
            {
                entity.ToTable("tipo_proceso", "siscb_core");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('siscb_core.tipo_proceso_id_seq'::regclass)");

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasColumnName("descripcion")
                    .HasColumnType("varchar");
            });

            modelBuilder.HasSequence("user_id_seq").StartsAt(7);

            modelBuilder.HasSequence("tipo_cliente_idtipocliente_seq");

            modelBuilder.HasSequence("Caja_id_seq");

            modelBuilder.HasSequence("cajacuentacontable_id_seq");

            modelBuilder.HasSequence("ceja_estado_nestado_seq");

            modelBuilder.HasSequence("conciliacion_bancaria_id_seq");

            modelBuilder.HasSequence("conciliacion_bancaria_iecb_id_seq");

            modelBuilder.HasSequence("cuentas_banco_username_id_seq");

            modelBuilder.HasSequence("ingresos_egresos_banco_estado_id_seq");

            modelBuilder.HasSequence("ingresos_egresos_banco_id_seq");

            modelBuilder.HasSequence("ingresos_egresos_caja_detalle_id_seq");

            modelBuilder.HasSequence("ingresos_egresos_caja_id_seq");

            modelBuilder.HasSequence("ingresos_egresos_caja_referencias_id_seq");

            modelBuilder.HasSequence("ingresos_egresos_caja_referencias_id_seq1");

            modelBuilder.HasSequence("loterecibos_Id_seq");

            modelBuilder.HasSequence("procesobanco_id_seq");

            modelBuilder.HasSequence("tipo_documento_tipo_doc_seq");

            modelBuilder.HasSequence("tipo_movimiento_idtipomovimiento_seq");

            modelBuilder.HasSequence("tipo_proceso_id_seq");

            modelBuilder.HasSequence("tipoingreso_idtipoingreso_seq");

            modelBuilder.HasSequence("tipomoneda_idtipomoneda_seq");

            modelBuilder.HasSequence("tipopago_id_seq");
        }
    }
}
