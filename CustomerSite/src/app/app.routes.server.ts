  import { RenderMode, ServerRoute } from '@angular/ssr';

  export const serverRoutes: ServerRoute[] = [
    {
      path: 'enterpin/:email/:expireAt',
      renderMode: RenderMode.Client
    },
    {
      path: 'resetpassword/:email',
      renderMode: RenderMode.Client
    },
    {
      path: 'ProductDetails/:ProductId',
      renderMode: RenderMode.Client
    },
    {
      path: 'editAddress/:id',
      renderMode: RenderMode.Client
    },
    {
      path: 'orderdetails/:id',
      renderMode: RenderMode.Client
    },
    {
      path: 'ProductsWithCategory/:id',
      renderMode: RenderMode.Client
    },
    {
      path: '**',
      renderMode: RenderMode.Prerender
    }
  ];
