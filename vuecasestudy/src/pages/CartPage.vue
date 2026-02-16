<template>
  <div class="text-center">
    <div class="text-h4 q-mt-md text-blue-grey-9">Cart Contents</div>

    <q-avatar class="q-mt-md" size="100px" square>
      <img :src="'/icons/CartIcon.png'" />
    </q-avatar>

    <div v-if="state.shoppingCart.length > 0" class="q-pa-md">
      <q-markup-table flat bordered class="q-mt-lg" style="max-width: 90vw; margin: 0 auto">
        <thead style="background-color: #37474f; color: white">
          <tr>
            <th class="text-left">Name</th>
            <th class="text-center">Qty</th>
            <th class="text-right">MSRP</th>
            <th class="text-right">Extended</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in state.shoppingCart" :key="item.id">
            <td class="text-left">{{ item.item.productName }}</td>
            <td class="text-center">{{ item.qty }}</td>
            <td class="text-right">${{ item.item.msrp.toFixed(2) }}</td>
            <td class="text-right">${{ (item.item.msrp * item.qty).toFixed(2) }}</td>
          </tr>
          <tr>
            <td colspan="3" class="text-right text-bold">Sub:</td>
            <td class="text-right">${{ state.subtotal.toFixed(2) }}</td>
          </tr>
          <tr>
            <td colspan="3" class="text-right text-bold">Tax (13%):</td>
            <td class="text-right">${{ state.tax.toFixed(2) }}</td>
          </tr>
          <tr style="background-color: #f5f5f5">
            <td colspan="3" class="text-right text-bold" style="color: #43a047">Total:</td>
            <td class="text-right text-bold" style="color: #43a047">
              ${{ state.total.toFixed(2) }}
            </td>
          </tr>
        </tbody>
      </q-markup-table>

      <q-btn label="Place Order" color="deep-orange-5" @click="saveCart" class="q-mt-md" />

      <q-btn
        label="Empty Cart"
        color="deep-orange-5"
        icon="remove_shopping_cart"
        @click="clearCart"
        class="q-mt-md"
      />
    </div>

    <div v-else class="text-blue text-subtitle1 q-mt-md">{{ state.status }}</div>
  </div>
</template>

<script>
import { reactive, onMounted } from 'vue'
import { poster } from '../utils/apiutil'
export default {
  name: 'CartPage',
  setup() {
    const TAX_RATE = 0.13

    const state = reactive({
      shoppingCart: [],
      subtotal: 0,
      tax: 0,
      total: 0,
    })

    const saveCart = async () => {
      let customer = JSON.parse(sessionStorage.getItem('customer'))
      let cart = JSON.parse(sessionStorage.getItem('shoppingCart'))

      try {
        state.status = 'Sending order to server...'

        if (!customer || !cart || cart.length === 0) {
          state.status = 'Cart or customer info is missing.'
          return
        }

        const products = cart.map((item) => ({
          qty: item.qty,
          product: item.item,
          sellingPrice: item.item.msrp,
        }))

        const cartHelper = {
          email: customer.email,
          products: products,
        }

        console.log('Posting to server:', cartHelper)

        let payload = await poster('order', cartHelper)

        console.log('Server response:', payload)

        if (typeof payload === 'string' && payload.toLowerCase().startsWith('order')) {
          clearCart()
        }

        if (typeof payload === 'string') {
          const match = payload.match(/^Order\s+\d+/i)
          state.status = match ? `${match[0]} created successfully.` : payload
        } else {
          state.status = 'Order created.'
        }
      } catch (err) {
        console.error('Caught error:', err)
        state.status = `Error saving order: ${err.message || err}`
      }
    }

    const calculateTotals = () => {
      state.subtotal = state.shoppingCart.reduce((sum, item) => sum + item.item.msrp * item.qty, 0)
      state.tax = state.subtotal * TAX_RATE
      state.total = state.subtotal + state.tax
    }

    const clearCart = () => {
      sessionStorage.removeItem('shoppingCart')
      state.shoppingCart = []
    }

    onMounted(() => {
      const storedCart = sessionStorage.getItem('shoppingCart')
      if (storedCart) {
        state.shoppingCart = JSON.parse(storedCart)
        calculateTotals()
      }
    })

    return {
      state,
      clearCart,
      saveCart,
    }
  },
}
</script>
