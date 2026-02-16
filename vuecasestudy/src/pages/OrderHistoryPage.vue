<template>
  <div class="text-center">
    <q-avatar class="q-mt-md" size="100px" square>
      <img :src="'/icons/CartIcon.png'" />
    </q-avatar>
    <div class="text-h4 q-mt-md text-blue-grey-9">Order History</div>
    <div class="text-h6 text-positive q-mt-sm">
      loaded {{ state.orders.length }} order<span v-if="state.orders.length !== 1">s</span>
    </div>

    <!-- Orders Table -->
    <div class="q-mt-md q-pa-sm bg-white shadow-2" style="max-width: 90vw; margin: auto">
      <q-item class="q-py-sm bg-grey-3">
        <q-item-section class="col-6 text-center text-bold">ID</q-item-section>
        <q-item-section class="col-6 text-center text-bold">Date</q-item-section>
      </q-item>

      <div v-for="order in state.orders" :key="order.id">
        <q-item clickable class="q-py-sm" @click="selectOrder(order.id)">
          <q-item-section class="col-6 text-center">{{ order.id }}</q-item-section>
          <q-item-section class="col-6 text-center">{{
            formatDate(order.orderDate)
          }}</q-item-section>
        </q-item>
      </div>
    </div>

    <q-dialog v-model="state.showDialog">
      <q-card style="min-width: 400px">
        <q-card-section class="text-center">
          <div class="text-h6">Order #{{ state.selectedOrderId }}</div>
          <div class="text-subtitle2">{{ state.selectedOrderDate }}</div>
        </q-card-section>

        <q-separator />

        <q-card-section>
          <q-markup-table flat bordered>
            <thead>
              <tr>
                <th class="text-left" rowspan="2">Name</th>
                <th class="text-center" colspan="3">Quantities</th>
                <th class="text-right" rowspan="2">Extended</th>
              </tr>
              <tr>
                <th class="text-center">S</th>
                <th class="text-center">O</th>
                <th class="text-center">B</th>
              </tr>
            </thead>

            <tbody>
              <tr v-for="item in state.selectedOrderItems" :key="item.productId">
                <td class="text-left">{{ item.productName }}</td>
                <td class="text-center">{{ item.qtySold }}</td>
                <td class="text-center">{{ item.qtyOrdered }}</td>
                <td class="text-center">{{ item.qtyBackOrdered }}</td>
                <td class="text-right">${{ item.sellingPrice.toFixed(2) }}</td>
              </tr>
              <tr>
                <td colspan="4" class="text-right text-bold">Sub:</td>
                <td class="text-right">${{ state.subtotal.toFixed(2) }}</td>
              </tr>
              <tr>
                <td colspan="4" class="text-right text-bold">Tax(13%):</td>
                <td class="text-right">${{ state.tax.toFixed(2) }}</td>
              </tr>
              <tr style="background-color: #f0f0f0">
                <td colspan="4" class="text-right text-bold text-primary">Total:</td>
                <td class="text-right text-bold text-primary">${{ state.total.toFixed(2) }}</td>
              </tr>
            </tbody>
          </q-markup-table>
        </q-card-section>

        <q-card-actions align="right">
          <q-btn flat label="Close" color="primary" v-close-popup />
        </q-card-actions>
      </q-card>
    </q-dialog>
  </div>
</template>

<script>
import { reactive, onMounted } from 'vue'
import { fetcher } from '../utils/apiutil'
import { formatDate } from '../utils/formatutils'

export default {
  name: 'OrderHistoryPage',
  setup() {
    const TAX_RATE = 0.13
    const customer = JSON.parse(sessionStorage.getItem('customer'))

    const state = reactive({
      orders: [],
      selectedOrderItems: [],
      selectedOrderId: null,
      selectedOrderDate: '',
      subtotal: 0,
      tax: 0,
      total: 0,
      showDialog: false,
    })

    onMounted(async () => {
      try {
        const payload = await fetcher(`order/${customer.email}`)
        state.orders = payload
      } catch (err) {
        console.error('Error loading orders:', err)
      }
    })

    const selectOrder = async (orderId) => {
      try {
        const payload = await fetcher(`order/${orderId}/${customer.email}`)
        state.selectedOrderId = orderId
        state.selectedOrderItems = payload
        state.selectedOrderDate = payload[0]?.orderDate || ''

        state.subtotal = payload.reduce((sum, item) => sum + item.sellingPrice * item.qtySold, 0)
        state.tax = state.subtotal * TAX_RATE
        state.total = state.subtotal + state.tax

        state.showDialog = true
      } catch (err) {
        console.error('Error loading order details:', err)
      }
    }

    return {
      state,
      selectOrder,
      formatDate,
    }
  },
}
</script>
