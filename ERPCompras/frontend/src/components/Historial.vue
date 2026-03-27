<template>
  <div class="historial-container">
    <h1>📋 Historial de Compras</h1>

    <!-- Filtros -->
    <div class="filtros">
      <label>Desde: <input type="date" v-model="fechaInicio" /></label>
      <label>Hasta: <input type="date" v-model="fechaFin" /></label>
      <button @click="cargarHistorial">🔍 Filtrar</button>
      <button @click="limpiarFiltros">✖ Limpiar</button>
    </div>

    <!-- Exportar -->
    <div class="exportar">
      <button @click="exportarExcel">📊 Exportar Excel</button>
      <button @click="exportarPDF">📄 Exportar PDF</button>
    </div>

    <!-- Loading -->
    <div v-if="cargando" class="loading">⏳ Cargando...</div>

    <!-- Error -->
    <div v-else-if="error" class="error">⚠️ {{ error }}</div>

    <!-- Tabla -->
    <div v-else-if="registros.length > 0">
      <table>
        <thead>
          <tr>
            <th>Fecha</th>
            <th>Proveedor</th>
            <th>Monto</th>
            <th>Estado</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in registros" :key="item.id">
            <td>{{ item.fecha }}</td>
            <td>{{ item.proveedor }}</td>
            <td>Q {{ Number(item.monto).toFixed(2) }}</td>
            <td>
              <span class="badge" :class="item.estado.toLowerCase()">
                {{ item.estado }}
              </span>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Sin registros (Criterio 5) -->
    <div v-else class="sin-registros">
      <p>📭 Sin historial disponible</p>
    </div>
  </div>
</template>

<script>
export default {
  name: 'HistorialCompras',
  data() {
    return {
      fechaInicio: '',
      fechaFin: '',
      registros: [],
      cargando: false,
      error: null
    }
  },
  mounted() {
    this.cargarHistorial()
  },
  methods: {
    async cargarHistorial() {
      this.cargando = true
      this.error = null
      try {
        const params = new URLSearchParams()
        if (this.fechaInicio) params.append('inicio', this.fechaInicio)
        if (this.fechaFin)    params.append('fin', this.fechaFin)

       const url = `http://127.0.0.1:5000/compras/historial?${params.toString()}`
        const response = await fetch(url)

        if (!response.ok) throw new Error('Error al obtener datos')

        this.registros = await response.json()
      } catch (e) {
        this.error = 'No se pudo conectar al servidor. Verifique que el backend esté corriendo.'
      } finally {
        this.cargando = false
      }
    },
    limpiarFiltros() {
      this.fechaInicio = ''
      this.fechaFin = ''
      this.cargarHistorial()
    },
    exportarExcel() {
      const encabezados = ['Fecha', 'Proveedor', 'Monto', 'Estado']
      const filas = this.registros.map(r =>
        [r.fecha, r.proveedor, Number(r.monto).toFixed(2), r.estado].join(',')
      )
      const csv = [encabezados.join(','), ...filas].join('\n')
      const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' })
      const url = URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.href = url
      link.download = 'historial_compras.csv'
      link.click()
      URL.revokeObjectURL(url)
    },
    exportarPDF() {
      window.print()
    }
  }
}
</script>

<style scoped>
.historial-container {
  max-width: 900px;
  margin: 30px auto;
  font-family: Arial, sans-serif;
  padding: 20px;
}
h1 { color: #2c3e50; margin-bottom: 20px; }

.filtros {
  display: flex;
  gap: 12px;
  align-items: flex-end;
  margin-bottom: 16px;
  flex-wrap: wrap;
}
.filtros label {
  display: flex;
  flex-direction: column;
  font-size: 13px;
  gap: 4px;
}
.filtros input {
  padding: 6px;
  border: 1px solid #ccc;
  border-radius: 4px;
}
.exportar {
  display: flex;
  gap: 10px;
  margin-bottom: 20px;
}
button {
  padding: 8px 14px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  background-color: #3498db;
  color: white;
  font-size: 13px;
}
button:hover { background-color: #2980b9; }

table { width: 100%; border-collapse: collapse; }
th {
  background-color: #2c3e50;
  color: white;
  padding: 10px;
  text-align: left;
}
td {
  padding: 10px;
  border-bottom: 1px solid #ddd;
}
tr:hover td { background-color: #f0f4f8; }

.badge {
  padding: 4px 10px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: bold;
  color: white;
}
.badge.pagado    { background-color: #27ae60; }
.badge.pendiente { background-color: #f39c12; }
.badge.cancelado { background-color: #e74c3c; }

.sin-registros {
  text-align: center;
  padding: 40px;
  color: #999;
  font-size: 18px;
  border: 2px dashed #ddd;
  border-radius: 8px;
}
.loading { text-align: center; padding: 30px; font-size: 16px; }
.error {
  text-align: center;
  padding: 20px;
  color: #e74c3c;
  border: 1px solid #e74c3c;
  border-radius: 8px;
}
</style>