<template>
  <div class="inventario-container">
    <h1>Inventario</h1>

    <button @click="cargarTodo">Recargar</button>

    <div v-if="error" class="error">{{ error }}</div>

    <table v-if="inventario.length">
      <thead>
        <tr>
          <th>Producto</th>
          <th>Stock</th>
          <th>Mínimo</th>
          <th>Estado</th>
          <th>Nuevo stock</th>
          <th>Nuevo mínimo</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="item in inventario" :key="item.id">
          <td>{{ item.nombre }}</td>
          <td>{{ item.stock }}</td>
          <td>{{ item.stock_minimo }}</td>
          <td>
            <span :class="item.stock < item.stock_minimo ? 'bajo' : 'normal'">
              {{ item.stock < item.stock_minimo ? 'Bajo' : 'Normal' }}
            </span>
          </td>
          <td>
            <input type="number" v-model.number="edicionStock[item.id]" />
            <button @click="guardarStock(item.id)">Guardar</button>
          </td>
          <td>
            <input type="number" v-model.number="edicionMinimo[item.id]" />
            <button @click="guardarMinimo(item.id)">Guardar</button>
          </td>
        </tr>
      </tbody>
    </table>

    <h2>Alertas</h2>
    <ul>
      <li v-for="alerta in alertas" :key="alerta.id">
        {{ alerta.mensaje }}
      </li>
    </ul>

    <h2>Solicitudes de compra</h2>
    <ul>
      <li v-for="sol in solicitudes" :key="sol.id">
        {{ sol.producto }} - {{ sol.estado }} - Cantidad sugerida: {{ sol.cantidad_sugerida }}
      </li>
    </ul>

    <h2>Notificaciones al jefe de compras</h2>
    <ul>
      <li v-for="n in notificaciones" :key="n.id">
        {{ n.mensaje }}
      </li>
    </ul>
  </div>
</template>

<script>
export default {
  name: 'InventarioView',
  data() {
    return {
      inventario: [],
      alertas: [],
      solicitudes: [],
      notificaciones: [],
      edicionStock: {},
      edicionMinimo: {},
      error: null
    }
  },
  mounted() {
    this.cargarTodo()
  },
  methods: {
    async cargarTodo() {
      this.error = null
      try {
        const [inventarioRes, alertasRes, solicitudesRes, notificacionesRes] = await Promise.all([
          fetch('http://127.0.0.1:5000/inventario'),
          fetch('http://127.0.0.1:5000/alertas-inventario'),
          fetch('http://127.0.0.1:5000/solicitudes-compra'),
          fetch('http://127.0.0.1:5000/notificaciones/jefe-compras')
        ])

        this.inventario = await inventarioRes.json()
        this.alertas = await alertasRes.json()
        this.solicitudes = await solicitudesRes.json()
        this.notificaciones = await notificacionesRes.json()

        this.inventario.forEach(item => {
          this.edicionStock[item.id] = item.stock
          this.edicionMinimo[item.id] = item.stock_minimo
        })
      } catch (e) {
        this.error = 'No se pudo cargar el módulo de inventario.'
      }
    },

    async guardarStock(productoId) {
      await fetch('http://127.0.0.1:5000/inventario/ajustar-stock', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          producto_id: productoId,
          stock: this.edicionStock[productoId]
        })
      })
      this.cargarTodo()
    },

    async guardarMinimo(productoId) {
      await fetch('http://127.0.0.1:5000/inventario/configurar-minimo', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          producto_id: productoId,
          stock_minimo: this.edicionMinimo[productoId]
        })
      })
      this.cargarTodo()
    }
  }
}
</script>

<style scoped>
.inventario-container {
  max-width: 1100px;
  margin: 30px auto;
  font-family: Arial, sans-serif;
  padding: 20px;
}
table {
  width: 100%;
  border-collapse: collapse;
  margin: 20px 0;
}
th, td {
  border-bottom: 1px solid #ddd;
  padding: 10px;
  text-align: left;
}
input {
  width: 90px;
  margin-right: 8px;
}
button {
  margin-top: 6px;
  padding: 6px 10px;
  border: none;
  background: #3498db;
  color: white;
  border-radius: 4px;
  cursor: pointer;
}
.bajo {
  color: #c0392b;
  font-weight: bold;
}
.normal {
  color: #27ae60;
  font-weight: bold;
}
.error {
  color: #c0392b;
  margin: 15px 0;
}
</style>