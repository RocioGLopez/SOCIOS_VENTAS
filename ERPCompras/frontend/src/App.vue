<template>
  <div>
    <Inventario />
    <hr style="margin: 40px 0;" />
    <Historial />
  </div>
</template>

<script>
import Inventario from './components/Inventario.vue'
import Historial from './components/Historial.vue'

export default {
  components: { Inventario, Historial }
}
</script>