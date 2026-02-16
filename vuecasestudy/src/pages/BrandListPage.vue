<template>
  <div class="text-center">
    <div class="row justify-center q-mt-md">
      <q-avatar size="80px" style="border-radius: 50%">
        <img :src="`/img/BrandLogo.png`" />
      </q-avatar>
    </div>

    <div class="text-h2 q-mt-lg">Brands</div>
    <div class="status q-mt-md text-subtitle2 text-negative mb-4">
      {{ state.status }}
    </div>

    <q-select
      class="q-mt-lg q-ml-lg"
      v-if="state.brands.length > 0"
      style="width: 50vw; margin-bottom: 4vh; background-color: #fff"
      :option-value="'id'"
      :option-label="'name'"
      :options="state.brands"
      label="Select a Brand"
      v-model="state.selectedBrandId"
      @update:model-value="getProducts()"
      emit-value
      map-options
    />
    <div class="text-h6 text-bold text-center text-primary" v-if="state.Products.length > 0">
      {{ state.selectedBrand.name }} ITEMS
    </div>
    <q-scroll-area style="height: 55vh">
      <q-card class="q-ma-md">
        <q-list separator>
          <q-item
            clickable
            v-for="product in state.Products"
            :key="product.id"
            @click="selectProduct(product.id)"
          >
            <q-item-section avatar>
              <q-avatar style="height: 125px; width: 90px" square>
                <img :src="`/img/${product.graphicName}`" />
              </q-avatar>
            </q-item-section>

            <q-item-section class="text-left">
              {{ product.productName }}
            </q-item-section>
          </q-item>
        </q-list>
      </q-card>
      <q-dialog v-model="state.productSelected" transition-show="rotate" transition-hide="rotate">
        <q-card style="min-width: 300px">
          <q-card-actions align="right">
            <q-btn flat label="X" color="primary" v-close-popup class="text-h5" />
          </q-card-actions>
          <q-card-section>
            <div class="row justify-center">
              <q-avatar style="height: 125px; width: 90px" square>
                <img :src="`/img/${state.selectedProduct.graphicName}`" />
              </q-avatar>
            </div>
            <div class="row justify-center q-mt-sm">
              <div class="text-subtitle2 text-center q-mr-sm">
                {{ state.selectedProduct.productName }} -
              </div>
              <div class="text-subtitle2 text-center text-primary">
                ${{ state.selectedProduct.msrp }}
              </div>
            </div>
          </q-card-section>
          <div class="row justify-center">
            <q-card-section>
              <q-chip icon="bookmark" color="primary" text-color="white"
                >Details
                <q-tooltip
                  transition-show="flip-right"
                  transition-hide="flip-left"
                  text-color="white"
                >
                  {{ state.selectedProduct.description }}.
                </q-tooltip>
              </q-chip>
            </q-card-section>
          </div>

          <q-card-section>
            <div class="row">
              <q-input
                v-model.number="state.qty"
                type="number"
                filled
                placeholder="qty"
                hint="Qty"
                dense
                style="max-width: 12vw"
              />&nbsp;
              <q-btn
                color="primary"
                label="Add To  Cart"
                :disable="state.qty < 0"
                @click="addToCart()"
                style="max-width: 25vw; margin-left: 3vw"
              />
              <q-btn
                color="secondary"
                label="View Cart"
                @click="viewCart()"
                style="max-width: 25vw; margin-left: 3vw"
              />
            </div>
          </q-card-section>

          <q-card-section class="text-center text-positive">
            {{ state.dialogStatus }}
          </q-card-section>
        </q-card>
      </q-dialog>
    </q-scroll-area>
  </div>
</template>

<script>
import { reactive, onMounted } from 'vue'
import { fetcher } from '../utils/apiutil'
import { useRouter } from 'vue-router'
export default {
  setup() {
    const router = useRouter()

    const viewCart = () => {
      router.push('cart')
    }

    const addToCart = () => {
      const storedCart = sessionStorage.getItem('shoppingCart')
      if (storedCart) {
        state.shoppingCart = JSON.parse(storedCart)
      }

      let index = -1

      if (state.shoppingCart.length > 0) {
        index = state.shoppingCart.findIndex((product) => product.id === state.selectedProduct.id)
      }

      if (state.qty > 0) {
        if (index === -1) {
          state.shoppingCart.push({
            id: state.selectedProduct.id,
            qty: state.qty,
            item: state.selectedProduct,
          })
        } else {
          state.shoppingCart[index] = {
            id: state.selectedProduct.id,
            qty: state.qty,
            item: state.selectedProduct,
          }
        }
        state.dialogStatus = `${state.qty} product(s) added`
      } else {
        if (index !== -1) {
          state.shoppingCart.splice(index, 1)
        }
        state.dialogStatus = `item(s) removed`
      }

      sessionStorage.setItem('shoppingCart', JSON.stringify(state.shoppingCart))
      state.qty = 0
    }

    const getProducts = async () => {
      try {
        state.selectedBrand = state.brands.find((brand) => brand.id === state.selectedBrandId)
        state.status = `finding Products for brand ${state.selectedBrand}...`
        state.Products = await fetcher(`Product/${state.selectedBrand.id}`)
        state.status = `loaded ${state.Products.length} menu items for    ${state.selectedBrand.name}`
      } catch (err) {
        console.log(err)
        state.status = `Error has occured: ${err.message}`
      }
    }
    const selectProduct = async (productid) => {
      try {
        state.selectedProduct = state.Products.find((product) => product.id === productid)
        state.productSelected = true
        state.dialogStatus = ''
        if (sessionStorage.getItem('Shopping Cart')) {
          state.tray = JSON.parse(sessionStorage.getItem('Shopping Cart'))
        }
      } catch (err) {
        console.log(err)
        state.status = `Error has occured: ${err.message}`
      }
    }

    let state = reactive({
      status: '',
      brands: [],
      Products: [],
      selectedBrand: {},
      selectedBrandId: '',
      selectedProduct: {},
      dialogStatus: '',
      productSelected: false,
      qty: 0,
      shoppingCart: [],
    })

    const loadBrands = async () => {
      try {
        state.status = 'Loading Brands ...'
        state.brands = await fetcher(`Brand`)
        state.status = 'Brands loaded successfully'
      } catch (err) {
        console.log(err)
        state.status = `Error has occured: ${err.message}`
      }
    }

    onMounted(() => {
      loadBrands()
    })

    return {
      loadBrands,
      state,
      getProducts,
      selectProduct,
      addToCart,
      viewCart,
    }
  },
}
</script>
