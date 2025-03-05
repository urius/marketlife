using System;
using System.Collections.Generic;
using System.Linq;
using Src.Common;
using Src.Model.Configs;
using Src.Model.Humans;
using Src.Systems;
using UnityEngine;

namespace Src.Model
{
    public class UserSessionDataModel
    {
        public event Action<CustomerModel> CustomerAdded = delegate { };
        public event Action<CustomerModel> CustomerRemoved = delegate { };

        public readonly Dictionary<ProductConfig, ProductInfoForBuy> ProductsData = new Dictionary<ProductConfig, ProductInfoForBuy>();

        public int SpawnCooldown = 0;

        private readonly List<CustomerModel> _customers = new List<CustomerModel>(Constants.MaxCustomersCount);
        private readonly Dictionary<Vector2Int, LinkedList<CustomerModel>> _customersByCoords = new Dictionary<Vector2Int, LinkedList<CustomerModel>>(Constants.MaxCustomersCount);

        public UserSessionDataModel()
        {
        }

        public IReadOnlyList<CustomerModel> Customers => _customers;

        public bool AddCustomer(CustomerModel model)
        {
            if (HaveCustomerAt(model.Coords))
            {
                return false;
            }
            _customers.Add(model);
            AddCustomerOnCoords(model, model.Coords);
            SubscribeForCustomer(model);
            CustomerAdded(model);

            return true;
        }

        public bool RemoveCustomer(CustomerModel model)
        {
            if (_customers.Remove(model))
            {
                RemoveCustomerFromCoords(model, model.Coords);
                UnsubscribeFromCustomer(model);
                CustomerRemoved(model);
                return true;
            }

            return false;
        }

        public bool HaveCustomerAt(Vector2Int coords)
        {
            if (_customersByCoords.TryGetValue(coords, out var result))
            {
                return result.Any();
            }
            return false;
        }

        public IEnumerable<CustomerModel> GetCustomersAt(Vector2Int coords)
        {
            if (_customersByCoords.TryGetValue(coords, out var result))
            {
                return result;
            }
            return Enumerable.Empty<CustomerModel>();
        }

        private void AddCustomerOnCoords(CustomerModel model, Vector2Int coords)
        {
            if (_customersByCoords.ContainsKey(coords) == false)
            {
                _customersByCoords[coords] = new LinkedList<CustomerModel>();
            }

            _customersByCoords[coords].AddLast(model);
        }

        private bool RemoveCustomerFromCoords(CustomerModel model, Vector2Int coords)
        {
            if (_customersByCoords.ContainsKey(coords))
            {
                return _customersByCoords[coords].Remove(model);
            }
            return false;
        }

        private void SubscribeForCustomer(CustomerModel model)
        {
            model.CoordsChanged += OnCustomerCoordsChanged;
        }

        private void UnsubscribeFromCustomer(CustomerModel model)
        {
            model.CoordsChanged -= OnCustomerCoordsChanged;
        }

        private void OnCustomerCoordsChanged(PositionableObjectModelBase model, Vector2Int previousCoords, Vector2Int currentCoords)
        {
            var customerModel = model as CustomerModel;
            RemoveCustomerFromCoords(customerModel, previousCoords);
            AddCustomerOnCoords(customerModel, customerModel.Coords);
        }
    }
}
