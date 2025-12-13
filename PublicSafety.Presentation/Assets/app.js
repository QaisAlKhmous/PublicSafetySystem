var app = angular.module('myApp', ["ngRoute", 'ngTable', 'ngCookies']);


app.config(function ($routeProvider) {



    $routeProvider
        .when("/", {
            templateUrl: '/Home/Dashboard',
            controller: 'dashboardCtrl'
        })
       .when("/items", {
            templateUrl: '/Home/Items',
            controller: 'itemsCtrl'

       })

        .when("/matrices", {
            templateUrl: '/Home/Matrices',
            controller: 'matrixCtrl'

        })
        .when("/employees", {
            templateUrl: '/Home/Employees',
            controller: 'employeeCtrl'

        })
        .when("/addEmployee", {
            templateUrl: '/Home/AddEmployee',
            controller: 'addEmployeeCtrl'

        })
        .when("/editEmployee/:employeeId", {
            templateUrl: '/Home/AddEmployee',
            controller: 'addEmployeeCtrl'

        })
        .when("/issuances/:employeeId", {
            templateUrl: '/Home/Issuances',
            controller: 'issuanceCtrl'
        })
        .when("/entitlements/:employeeId", {
            templateUrl: '/Home/Entitlements',
            controller: 'entitlementCtrl'
        })
        .when("/itemLogs/:itemId", {
            templateUrl: '/Home/ItemLogs',
            controller: 'itemLogCtrl'
        })
       


});



app.run(function ($rootScope, $location, $cookies,$timeout) {

    $rootScope.CurrentUser = $cookies.get('UserInfo');


    if ($rootScope.CurrentUser == null) {

        window.location = "/#!/"
        return;
    }


    var res = $rootScope.CurrentUser.split("&");

    $rootScope.LogedInUser = { username: '', userId: '', userType: '' };

    $rootScope.LogedInUser.username = (res[0]).replace("Username=", "");
    $rootScope.LogedInUser.userId = (res[1]).replace("UserId=", "");
    $rootScope.LogedInUser.userType = (res[2]).replace("Type=", "");



    
    $rootScope.toastify = function (msg, type) {
        if (type)
            color = "#4fbe87"
        else
            color = "#FF0000";
        if (typeof Toastify !== "undefined") {
            Toastify({
                text: msg,
                duration: 3000,
                close: true,
                gravity: "bottom",
                position: "center",
                backgroundColor: color
            }).showToast();
        }
    };


  
    $rootScope.initFileDropzone = function (elementId, model, fieldName) {

        Dropzone.autoDiscover = false;

        var dz = new Dropzone(elementId, {
            url: "/Upload/UploadFile",
            paramName: "file",
            maxFiles: 1,
            addRemoveLinks: true,
            acceptedFiles: ".jpeg,.jpg,.png,.gif,.pdf,.doc,.msg",
            dictDefaultMessage: "ارفع نموذج الاستلام",

            init: function () {
                var dropzone = this; // ✅ CORRECT INSTANCE

                dropzone.on("success", function (file, response) {
                    $rootScope.$applyAsync(function () {
                        model[fieldName] = response.fileName;
                    });
                });

                dropzone.on("removedfile", function () {
                    $rootScope.$applyAsync(function () {
                        model[fieldName] = '';
                    });
                });
            }
        });

        return dz;
    };

    $rootScope.goBackToEmployees = function () {
        $location.path('/employees');
    };

    $rootScope.goBackToItems = function () {
        $location.path('/items');
    };
})

app.factory('itemService', function ($http) {
    var baseUrl = '/Item'; // MVC controller base URL

    return {
        getItems: function () {
            
            return $http.get(baseUrl + '/GetItems');
        },
        getItemById: function (id) {
            return $http.get(baseUrl + '/GetItemById?Id=' + id);
        },
        addItem: function (item) {
            return $http.post(baseUrl + '/AddItem', item);
        },
        updateItem: function (item) {
            return $http.post(baseUrl + '/UpdateItem', item);
        },
        deleteItem: function (id) {
            return $http.post(baseUrl + '/DeleteItem', { id: id });
        },
        increaseQuantity: function (id, quantity,createdBy) {
            return $http.post(baseUrl + '/IncreaseQuantity', { id: id, quantity: quantity, CreatedBy: createdBy });
        },
        getNumberOfAllItems: function () {
            return $http.get(baseUrl + '/GetNumberOfAllItems');
        }
    };
});



app.factory('matrixService', function ($http) {
    var baseUrl = '/Matrix'; // MVC controller base URL

    return {
        getAllMatrices: function () {

            return $http.get(baseUrl + '/GetAllMatrices');
        },
        getItemById: function (id) {
            return $http.get(baseUrl + '/GetItemById?id=' + id);
        },
        addItem: function (item) {
            return $http.post(baseUrl + '/AddItem', item);
        },
        updateItem: function (item) {
            return $http.post(baseUrl + '/UpdateItem', item);
        },
        deleteItem: function (id) {
            return $http.post(baseUrl + '/DeleteItem', { id: id });
        }
    };
});

app.factory('disposalService', function ($http) {
    var baseUrl = '/Disposal'; // MVC controller base URL

    return {
        addDisposal: function (disposal) {
            return $http.post(baseUrl + '/AddDisposal', disposal);
        }
    };
});

app.factory('employeeService', function ($http) {
    var baseUrl = '/Employee'; // MVC controller base URL

    return {
        getEmployeeById: function (id) {

            return $http.get(baseUrl + '/GetEmployee?Id=' + id);
        },
        addNewEmployee: function (employee) {
            return $http.post(baseUrl + '/AddNewEmployee' , employee);
        },
        updateEmployee: function (employee) {
            return $http.post(baseUrl + '/UpdateEmployee', employee);
        },
        getAllEmployees: function () {
            return $http.get(baseUrl + '/GetAllEmployees');
        },
        deleteEmployee: function (id) {
            return $http.post(baseUrl + '/ResignEmployee', { Id: id })
        },
        getNumberOfActiveEmployees: function () {
            return $http.get(baseUrl + '/GetNumberOfActiveEmployees');
        },
        getNumberOfInactiveEmployees: function () {
            return $http.get(baseUrl + '/GetNumberOfInactiveEmployees');
        }
    };
});

app.controller('sidebarCtrl', function ($scope, $rootScope, $location) {
    $scope.IsAdmin = $rootScope.LogedInUser.userType == 0;
    $scope.activeItem = 0;

    $rootScope.setActive = function (item) {
        $scope.activeItem = item;

    }

    $rootScope.$on('$locationChangeSuccess', function (event, newUrl, oldUrl) {
        let url = $location.path();

        if (url === '/') {
            $rootScope.setActive(0);

        }


        if (url === '/matrices') {
            $rootScope.setActive(1);

        }
        if (url === '/employees' || url === '/addEmployee' || url.startsWith('/editEmployee') || url.startsWith("/issuances") || url.startsWith("/entitlements")) {
            $rootScope.setActive(2);

        }

        if (url === '/items') {
            $rootScope.setActive(3);

        }
    

    });
})

app.controller('itemsCtrl', function ($scope, NgTableParams, itemService, disposalService, $rootScope,$http) {

    $scope.items = [];
    $scope.newItem = { Name: '', Description: '', Quantity: 0, AddedBy: $rootScope.LogedInUser.username }
    $scope.clicked = false;
    $scope.selectedItem = null;  
    $scope.addedQuantity = 0;
    $scope.disposal = { ItemId: null, Quantity: 0, DisposalDate: '', DisposalFormPath: '', CreatedBy: $rootScope.LogedInUser.username , ApprovedBy: "Admin"}

   

    $scope.loadItems = function () {
        itemService.getItems().then(function (response) {
            $scope.items = response.data;
            
            $scope.itemsTableParams.settings({ dataset: $scope.items });
            

        }, function (error) {
            console.error("Error loading items", error);
        });
    };

    $scope.itemsTableParams = new NgTableParams(
        {
            page: 1,            // start on first page
            count: 10,          // items per page
            filter: {},
            sorting: { Name: "asc" }// initial filter
        }
    );
    $scope.itemsTableParams.settings().counts = [];
    $scope.loadItems();

    $scope.changeRequest = {
        EntityType: 'Item', EntityId: '', OldValue: null,
        NewValue: '', ChangedBy: $rootScope.LogedInUser.username
    }

    $scope.addNewItem = function () {
        $scope.additemForm.$setSubmitted();

        if (!$scope.newItem.Name) {

            console.warn("Form invalid");
            return;
        }
        if ($rootScope.LogedInUser.userType != 0) {
            $scope.changeRequest.NewValue = JSON.stringify($scope.newItem);
            $http.post('/ChangeRequest/AddNewChangeRequest', { ChangeRequest: $scope.changeRequest }).then((res) => {
                $rootScope.toastify('تم حفظ طلب التعديل بنجاح, وسيتم مراجعته من الادارة', 1);

            })

        }
        else {
            itemService.addItem($scope.newItem).then(function (response) {
                if (!response.data.success)
                    $rootScope.toastify('يوجد مادة بهذا الاسم', 0);
                else {
                    $scope.loadItems();
                    $rootScope.toastify('تم اضافة المادة بنجاح', 1);
                }

            }, function (error) {
                console.error("Error loading items", error);
            });

        }
        
      
    }

    $scope.confirmDelete = function (ItemId) {
        Swal.fire({
            title: 'هل انت متأكد من حذف المادة؟',
            text: "",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'نعم',
            cancelButtonText: 'لا'
        }).then((result) => {
            if (result.isConfirmed) $scope.deleteItem(ItemId);
        });
    }
   

    $scope.deleteItem = function (ItemId) {

        if ($rootScope.LogedInUser.userType != 0) {
            $scope.changeRequest.EntityId = ItemId;
            $http.post('/ChangeRequest/AddNewChangeRequest', { ChangeRequest: $scope.changeRequest }).then((res) => {
                $rootScope.toastify('تم حفظ الطلب بنجاح, وسيتم مراجعته من الادارة', 1);

            })

        } else {
            itemService.deleteItem(ItemId).then(function (response) {
                $scope.loadItems();
                $rootScope.toastify("تم حذف مادة بنجاح", 1)
            }, function (error) {
                console.error("Error deleting items", error);
            });
        }
      
    }

    $scope.changeSelectedItem = function (item) {
        $scope.selectedItem = item;
    }
   

    $scope.increaseQuantity = function () {
        if ($rootScope.LogedInUser.userType != 0) {
            $scope.changeRequest.EntityId = $scope.selectedItem.ItemId;
            var item = { Quantity: $scope.addedQuantity, IsIncrease: true };
            $scope.changeRequest.OldValue = JSON.stringify(item);

            $http.post('/ChangeRequest/AddNewChangeRequest', { ChangeRequest: $scope.changeRequest }).then((res) => {
                $rootScope.toastify('تم حفظ الطلب بنجاح, وسيتم مراجعته من الادارة', 1);

            })

        } else {
            itemService.increaseQuantity($scope.selectedItem.ItemId, $scope.addedQuantity, $rootScope.LogedInUser.username).then(function (response) {
                $scope.loadItems();
                $rootScope.toastify("تم زيادة رصيد مادة بنجاح", 1);
            }, function (error) {
                console.error("Error deleting items", error);
            });
        }
       
    }

    $(document).ready(function () {
        // Initialize disposal object if not already
        var scope = angular.element($('input[name="date"]')).scope();
        scope.$apply(function () {
            scope.disposal = scope.disposal || {};
            scope.disposal.DisposalDate = moment().format('YYYY-MM-DD'); // set default
      });

        $('input[name="date"]').daterangepicker({
            singleDatePicker: true,
            showDropdowns: true,
            autoUpdateInput: true, // input shows default
            startDate: moment(),   // today
            locale: { format: 'YYYY-MM-DD' }
        });

        $('input[name="date"]').on('apply.daterangepicker', function (ev, picker) {
            $(this).val(picker.startDate.format('YYYY-MM-DD'));

            var scope = angular.element(this).scope();
            scope.$apply(function () {
                scope.disposal.DisposalDate = picker.startDate.format('YYYY-MM-DD');
            });
        });

        $('input[name="date"]').on('cancel.daterangepicker', function (ev, picker) {
            $(this).val('');
            var scope = angular.element(this).scope();
            scope.$apply(function () {
                scope.disposal.DisposalDate = '';
            });
        });
    });

    var disposeDropzone = null;

    // =====================
    // Init Dropzone on modal open
    // =====================
    $('#disposeItemModalForm').on('shown.bs.modal', function () {
        if (!disposeDropzone) {
            initDisposeDropzone();
        }
    });

    // =====================
    // Clear Dropzone on modal close
    // =====================
    $('#disposeItemModalForm').on('hidden.bs.modal', function () {
        $scope.$apply(function () {

            if (disposeDropzone) {
                disposeDropzone.removeAllFiles(true);
            }

            $scope.disposal = {
                Quantity: 1,
                DisposalDate: '',
                DisposalFormPath: ''
            };

            if ($scope.disposeForm) {
                $scope.disposeForm.$setPristine();
                $scope.disposeForm.$setUntouched();
            }
        });
    });

    // =====================
    // Dropzone init
    // =====================
    function initDisposeDropzone() {

        Dropzone.autoDiscover = false;

        disposeDropzone = new Dropzone("#disposeFormDropzone", {
            url: "/Upload/UploadFile",
            paramName: "file",
            maxFiles: 1,
            addRemoveLinks: true,
            acceptedFiles: ".jpeg,.jpg,.png,.gif,.pdf,.doc,.msg",
            dictDefaultMessage: "ارفع نموذج التوقيع",

            init: function () {
                var dz = this;

                dz.on("success", function (file, response) {
                    $scope.$apply(function () {
                        $scope.disposal.DisposalFormPath = response.fileName;
                    });
                });

                dz.on("removedfile", function () {
                    $scope.$apply(function () {
                        $scope.disposal.DisposalFormPath = '';
                    });
                });
            }
        });
    }

    $scope.dispose = function () {
        $scope.disposeForm.$setSubmitted();

        if ($scope.disposeForm.$invalid) {
            return;
        }

        $scope.disposal.ItemId = $scope.selectedItem.ItemId;
        if ($scope.selectedItem.Quantity < $scope.disposal.Quantity) {
            $rootScope.toastify("الكمية المراد اتلافها غير متوفرة", 0)
        } else {
            if ($rootScope.LogedInUser.userType != 0) {
                $scope.changeRequest.EntityId = $scope.selectedItem.ItemId;
                var item = { Quantity: $scope.disposal.Quantity, IsIncrease: false };
                $scope.changeRequest.OldValue = JSON.stringify(item);

                $http.post('/ChangeRequest/AddNewChangeRequest', { ChangeRequest: $scope.changeRequest }).then((res) => {
                    $rootScope.toastify('تم حفظ الطلب بنجاح, وسيتم مراجعته من الادارة', 1);

                })

            }
            else {

                disposalService.addDisposal($scope.disposal).then(function (response) {
                    $scope.loadItems();
                    $rootScope.toastify("تم اتلاف مادة بنجاح", 1)
                }, function (error) {
                    console.error("Error deleting items", error);
                })
            }


        }
       
    }
})

app.controller('matrixCtrl', function ($scope, NgTableParams, matrixService, $http, $rootScope, itemService) {

    $scope.selectedCategory = null;

    
    $scope.loadCategories = function () {
        $http.get('/Category/GetAllCategories').then((res) => {
            $scope.Categories = res.data;
            $scope.selectedCategory = $scope.Categories[0];
            $scope.selectedCategoryChanged();
            console.log(res.data)
        })

    }
    $scope.loadCategories();


   
    $scope.loadItemsByCategory = function (CategoryId) {
        $http.get('/Matrix/GetItemsByCategory?CategoryId=' + CategoryId).then((res) => {

            if (res.data.error === 1)
                return 'لا يوجد مصفوفات لهذه الفئة';
            $scope.matrixItems = res.data;
           
        })
    }

    $scope.selectedCategoryChanged = function () {

        $scope.matrixItems = [];
        if ($scope.selectedCategory) {
            $scope.IsMatrixExistsByCategoryId($scope.selectedCategory.CategoryId);
           
        } else {
            $scope.matrixExists = false;
        }
    };

    $scope.IsMatrixExistsByCategoryId = function (CategoryId) {
        if (CategoryId == null)
            return false;
        $http.get('/Matrix/IsMatrixExistsByCategoryId?CategoryId=' + CategoryId).then((res) => {

            $scope.matrixExists = res.data;
            console.log($scope.matrixExists)
            if ($scope.matrixExists) {
                $scope.loadItemsByCategory($scope.selectedCategory.CategoryId)
            }
        })
    }

    $scope.confirmCreateMatrix = function () {
        Swal.fire({
            title: 'هل انت متأكد من اضافة مصفوفة جديدة؟',
            text: "",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'نعم',
            cancelButtonText: 'لا'
        }).then((result) => {
            if (result.isConfirmed) $scope.createMatrix();
        });
    }

    $scope.addMatrixItem = { MatrixId: null, ItemId: null, Quantity: 1, Frequency: 1, CreatedBy: $rootScope.LogedInUser.username };

    $scope.createMatrix = function () {
        $http.post('/Matrix/AddNewMatrix', { CategoryId: $scope.selectedCategory.CategoryId }).then((res) => {
            $rootScope.toastify('تم اضافة مصفوفة جديدة بنجاح', 1)
            $scope.matrixExists = true;
            $scope.loadItemsByCategory($scope.selectedCategory.CategoryId);
        })
    }

    $scope.addNewItem = function () {
        $http.post('/Matrix/AddItemInMatrix', { MatrixItem: $scope.addMatrixItem }).then((res) => {
            $rootScope.toastify('تم اضافة مادة جديدة الى المصفوفة بنجاح',1)
            $scope.loadItemsByCategory($scope.selectedCategory.CategoryId);
        })
    } 

    $scope.loadAddItem = function () {
        $http.get('/Matrix/GetMatrixByCategory?CategoryId=' + $scope.selectedCategory.CategoryId).then((res) => {
            $scope.addMatrixItem.MatrixId = res.data.MatrixId;
        })
        itemService.getItems().then((res) => {
            $scope.items = res.data;
        })
    }

    
    $scope.updateMatrixItem = { MatrixItemId: null, Frequency: 0, Quantity : 0}
    $scope.loadUpdateMatrixItem = function (matrixItem) {
        $scope.updateMatrixItem.MatrixItemId = matrixItem.MatrixItemId;
        $scope.updateMatrixItem.Frequency = matrixItem.Frequency;
        $scope.updateMatrixItem.Quantity = matrixItem.Quantity;
    }

  

    $scope.confirmDelete = function (matrixItemId) {
        Swal.fire({
            title: 'هل انت متأكد من حذف المادة؟',
            text: "",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'نعم',
            cancelButtonText: 'لا'
        }).then((result) => {
            if (result.isConfirmed) $scope.deleteMatrixItem(matrixItemId);
        });
       
    }

    $scope.deleteMatrixItem = function (matrixItemId) {
        $http.post('/Matrix/DeleteMatrixItem', {MatrixItemId: matrixItemId}).then((res) => {
            $rootScope.toastify('تم حذف المادة من المصفوفة بنجاح')
            $scope.loadItemsByCategory($scope.selectedCategory.CategoryId);
        })
    }

    $scope.updateMatrixItemFun = function () {
        $http.post('/Matrix/UpdateMatrixItem', { MatrixItem: $scope.updateMatrixItem }).then((res) => {
            $rootScope.toastify('تم تعديل المادة بنجاح',1)
            $scope.loadItemsByCategory($scope.selectedCategory.CategoryId);
        })
    }
  
    $scope.isEdit = false;
})
app.directive('fileModel', ['$parse', function ($parse) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var model = $parse(attrs.fileModel);
            var modelSetter = model.assign;

            element.bind('change', function () {
                scope.$apply(function () {
                    modelSetter(scope, element[0].files[0]);
                });
            });
        }
    };
}]);

app.controller('employeeCtrl', function ($scope, $timeout,NgTableParams, employeeService, $location, itemService, $http, $rootScope) {

    $scope.employees = [];

    $scope.searchBy = 0;

    $scope.downloadTemplate = function () {
        window.location = "/Upload/DownloadEmployeeTemplate";
    };

    $scope.uploadExcel = function () {
        if (!$scope.excelFile) {
            $rootScope.toastify('Please select a file', 0);
            return;
        }

        var formData = new FormData();
        formData.append('file', $scope.excelFile);

        $http.post('/Upload/UploadEmployeesExcel', formData, {
            transformRequest: angular.identity,
            headers: { 'Content-Type': undefined }
        }).then(function (res) {
            $scope.uploadResult = res.data;
            $rootScope.toastify('Upload completed', 1);
        }).catch(function (err) {
            $rootScope.toastify('Upload failed', 0);
            console.error(err);
        });
    };


    $scope.getAllEmployees = function () {
        employeeService.getAllEmployees().then(function (response) {
            $scope.employees = response.data;

            $scope.employeesTableParams.settings({ dataset: $scope.employees });
        })
    }

    $scope.getAllEmployees();


    $scope.employeesTableParams = new NgTableParams(
        {
            page: 1,            // start on first page
            count: 10,          // items per page
            filter: {},
            sorting: { Name: "asc" }// initial filter
        }
    );
    $scope.employeesTableParams.settings().counts = [];

    $scope.GoToAddPage = function () {
        $location.path('/addEmployee');
    }

    // Handle Issue Exception

    $scope.issuance = { EmployeeId: null, IssuanceId: null, ItemId: null, Quantity: 1, Type: '', ExceptionReason: '', ExceptionFormPath: '', SignedReceiptPath:'' ,CreatedBy: $rootScope.LogedInUser.username,IssuanceDate : '' };

    $scope.loadException = function (EmployeeId) {
        $scope.loadItems();
        $scope.issuance.Type = 'Exception';
        $scope.msg = 'تم اضافة استثناء مادة بنجاح'
        $scope.issuance.EmployeeId = EmployeeId;
    }

    $scope.loadDamaged = function (EmployeeId) {
        $scope.loadItems();
        $scope.issuance.Type = 'Damaged';
        $scope.msg = 'تم اضافة بدل تلف مادة بنجاح'
        $scope.issuance.EmployeeId = EmployeeId;
    }


    $scope.loadItems = function () {
        itemService.getItems().then((res) => {
            $scope.items = res.data;
        })
    }
    $scope.changeRequest = {
        EntityType: 'Issuance', EntityId: '', OldValue: null,
        NewValue: '', ChangedBy: $rootScope.LogedInUser.username
    }

    $scope.addChangeRequest = function () {
        $scope.issuance.IssuanceDate = new Date().toISOString().split("T")[0];
        const { IssuanceId, ...issuanceRequest } = $scope.issuance;
        $scope.issuanceRequest = issuanceRequest;

        $scope.changeRequest.NewValue = JSON.stringify($scope.issuanceRequest);
        $http.post('/ChangeRequest/AddNewChangeRequest', { ChangeRequest: $scope.changeRequest }).then((res) => {
            $rootScope.toastify('تم حفظ طلب التعديل بنجاح, وسيتم مراجعته من الادارة', 1);

        })

    }

    $scope.issue = function () {

        if ($rootScope.LogedInUser.userType != 0) {
            $http.post('/Item/IsQuantityEnough', { Id: $scope.issuance.ItemId, Quantity: $scope.issuance.Quantity }).then((res) => {
                if (res.data)
                    $scope.addChangeRequest();
                else {
                    $rootScope.toastify('الكمية المطلوبة غير متوفرة', 0);
                }
            })
           
        } else {

            $http.post('/Issuance/AddNewIssuance', $scope.issuance)
                .then(function (res) {

                    if (res.data.success === false) {
                        $rootScope.toastify(res.data.message, 0);
                    } else {
                        $rootScope.toastify(res.data.message, 1);

                        $scope.issuance = {
                            EmployeeId: null,
                            IssuanceId: null,
                            ItemId: null,
                            Quantity: 1,
                            Type: '',
                            ExceptionReason: '',
                            ExceptionFormPath: '',
                            CreatedBy: $rootScope.LogedInUser.username
                        };
                        $scope.exceptionForm.$submitted = false;
                    }
                })
                .catch(function (err) {

                    $rootScope.toastify(err.data ? err.data.message : 'Something went wrong!', 0);
                });
        }


    }


    $scope.exceptionDropzone = null;
    $scope.damageDropzone = null;

    // =====================
    // Exception Modal
    // =====================
    $('#exceptionModalForm').on('shown.bs.modal', function () {
        if (!$scope.exceptionDropzone) {
            $scope.exceptionDropzone =
                $rootScope.initFileDropzone(
                    "#exceptionFormDropzone",
                    $scope.issuance,
                    "ExceptionFormPath"
                );
        }
    });

    $('#exceptionModalForm').on('hidden.bs.modal', function () {
        $scope.$apply(function () {

            if ($scope.exceptionDropzone) {
                $scope.exceptionDropzone.removeAllFiles(true);
            }

            $scope.issuance.ExceptionFormPath = '';

            if ($scope.exceptionForm) {
                $scope.exceptionForm.$setPristine();
                $scope.exceptionForm.$setUntouched();
            }
        });
    });

    // =====================
    // Damage Modal
    // =====================
    $('#damageModalForm').on('shown.bs.modal', function () {
        if (!$scope.damageDropzone) {
            $scope.damageDropzone =
                $rootScope.initFileDropzone(
                    "#damageFormDropzone",
                    $scope.issuance,
                    "SignedReceiptPath"
                );
        }
    });

    $('#damageModalForm').on('hidden.bs.modal', function () {
        $scope.$apply(function () {

            if ($scope.damageDropzone) {
                $scope.damageDropzone.removeAllFiles(true);
            }

            $scope.issuance.SignedReceiptPath = '';

            if ($scope.damageForm) {
                $scope.damageForm.$setPristine();
                $scope.damageForm.$setUntouched();
            }
        });
    });

    // =====================
    // Submit Exception
    // =====================
    $scope.issueException = function () {
        $scope.exceptionForm.$setSubmitted();

        if ($scope.exceptionForm.$invalid) {
            return;
        }

        $scope.issue();
    };

    // =====================
    // Submit Damage
    // =====================
    $scope.issueDamage = function () {
        $scope.damageForm.$setSubmitted();

        if ($scope.damageForm.$invalid) {
            return;
        }

        $scope.issue();
    };

    $scope.confirmDelete = function (EmployeeId) {
        Swal.fire({
            title: 'هل انت متأكد من تغيير حالة الموظف الى مستقيل؟',
            text: "",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'نعم',
            cancelButtonText: 'لا'
        }).then((result) => {
            if (result.isConfirmed) $scope.deleteEmployee(EmployeeId);
        });
    }

    $scope.deleteEmployee = function (EmployeeId) {
        employeeService.deleteEmployee(EmployeeId).then((res) => {
            $rootScope.toastify('تم تغيير حالة الموظف الى مستقيل', 1);
            $scope.getAllEmployees();
        })
    }
   
})
app.directive('arabicOnly', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ngModel) {
            ngModel.$parsers.push(function (value) {
                if (!value) return value;

                // يسمح فقط بالحروف العربية والمسافات
                var clean = value.replace(/[^\u0600-\u06FF\s]/g, '');

                if (clean !== value) {
                    ngModel.$setViewValue(clean);
                    ngModel.$render();
                }

                return clean;
            });
        }
    };
});

app.directive('englishOnly', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ngModel) {
            ngModel.$parsers.push(function (value) {
                if (!value) return value;

                // يسمح فقط بالأحرف الإنجليزية (A-Z, a-z) والمسافات
                var clean = value.replace(/[^a-zA-Z\s]/g, '');

                if (clean !== value) {
                    ngModel.$setViewValue(clean);
                    ngModel.$render();
                }

                return clean;
            });
        }
    };
});

app.directive('numbersOnly', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ngModel) {
            ngModel.$parsers.push(function (value) {
                if (!value) return value;

                // يسمح فقط بالأرقام 0-9
                var clean = value.replace(/[^0-9]/g, '');

                if (clean !== value) {
                    ngModel.$setViewValue(clean);
                    ngModel.$render();
                }

                return clean;
            });
        }
    };
});
app.directive('validEmail', function () {
    var EMAIL_REGEXP = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;

    return {
        require: 'ngModel',
        link: function (scope, element, attrs, ngModel) {
            ngModel.$parsers.push(function (value) {
                if (!value) return value;

                // يتحقق من صحة الإيميل
                if (EMAIL_REGEXP.test(value)) {
                    ngModel.$setValidity('email', true);
                } else {
                    ngModel.$setValidity('email', false);
                }

                return value;
            });
        }
    };
});

app.controller('addEmployeeCtrl', function ($scope, NgTableParams, employeeService, $location, $http, $timeout, $routeParams,$rootScope) {

   

    $scope.GoToListPage = function () {
        $location.path('/employees');
    }

    $(document).ready(function () {
        // Initialize disposal object if not already
        var scope = angular.element($('input[name="date"]')).scope();
        scope.$apply(function () {
            scope.employee.EmploymentDate = scope.employee.EmploymentDate || {};
            scope.employee.EmploymentDate = moment().format('YYYY-MM-DD'); // set default
        });

        $('input[name="date"]').daterangepicker({
            singleDatePicker: true,
            showDropdowns: true,
            autoUpdateInput: true, // input shows default
            startDate: moment(),   // today
            locale: { format: 'YYYY-MM-DD' }
        });

        $('input[name="date"]').on('apply.daterangepicker', function (ev, picker) {
            $(this).val(picker.startDate.format('YYYY-MM-DD'));

            var scope = angular.element(this).scope();
            scope.$apply(function () {
                scope.employee.EmploymentDate = picker.startDate.format('YYYY-MM-DD');
            });
        });

        $('input[name="date"]').on('cancel.daterangepicker', function (ev, picker) {
            $(this).val('');
            var scope = angular.element(this).scope();
            scope.$apply(function () {
                scope.employee.EmploymentDate = '';
            });
        });
    });


   

    $scope.loadJobTitles = function () {
        $http.get('/JobTitle/GetAllJobTitles')
            .then((res) => {
                $scope.JobTitles = res.data;
            }).catch((rej) => console.log(rej))
    }


    $scope.loadDepartments = function () {
        $http.get('/Department/GetAllDepartments')
            .then((res) => {
                $scope.Departments = res.data;
            }).catch((rej) => console.log(rej))
    }

    $scope.loadSections = function () {
        $http.get('/Section/GetAllSections')
            .then((res) => {
                $scope.Sections = res.data;
            }).catch((rej) => console.log(rej))
    }

    $scope.loadCategory = function (JobTitleId) {
        $http.get('/Category/GetCategoryByJobTitleId?JobTitleId=' + JobTitleId).then((res) => {
            $scope.jobTitleCategory = res.data;
           
        })
    }

    
    $scope.loadSections();
    $scope.loadDepartments();
    $scope.loadJobTitles();

    $scope.employee = {
        EmployeeId : null,FullName : '',FirstName: '', SecondName: '', LastName: '', Email: '', Phone: '', IsIntern: false, Notes: '', WorkLocation: 'Amman',
        HealthInsuranceFile: '', DepartmentId: null, SectionId: null, JobTitleId: null, CategoryId: null,
        EmploymentDate: '',Notes: '',Active: true
    }

    $scope.updateEmployeeId = $routeParams.employeeId;

    $scope.changeRequest = {
        EntityType: 'Employee', EntityId: '', OldValue: null,
        NewValue: '', ChangedBy: $rootScope.LogedInUser.username
    }

   
    $scope.checkIfUpdate = function () {
        if ($scope.updateEmployeeId) {
            console.log("hi");
            employeeService.getEmployeeById($scope.updateEmployeeId)
                .then((res) => {
                  
                    $scope.employee = res.data;
                    $scope.changeRequest = {
                        EntityType: 'Employee', EntityId: $scope.updateEmployeeId, OldValue: JSON.stringify($scope.employee),
                        NewValue: '', ChangedBy: $rootScope.LogedInUser.username
                    }

                    // Init dropzone AFTER employee is loaded
                    initHealthInsuranceDropzone();
                })
        }
    }
    $scope.checkIfUpdate();

    $scope.saveEmployee = function () {
        $scope.employeeForm.$setSubmitted();

        if ($scope.employeeForm.$invalid || !$scope.employee.JobTitleId || !$scope.employee.DepartmentId || !$scope.employee.SectionId) {

            console.warn("Form invalid");
            return;
        }
        if ($rootScope.LogedInUser.userType != 0) {

            const { EmployeeId, CategoryId, ...employeeRequest } = $scope.employee;
            $scope.employeeRequest = employeeRequest;

            $scope.changeRequest.NewValue = JSON.stringify($scope.employeeRequest);
            $http.post('/ChangeRequest/AddNewChangeRequest', { ChangeRequest: $scope.changeRequest }).then((res) => {
                $rootScope.toastify('تم حفظ طلب التعديل بنجاح, وسيتم مراجعته من الادارة', 1);

            })

        } else {
            if ($scope.updateEmployeeId) {


                employeeService.updateEmployee($scope.employee)
                    .then((res) => {
                        $rootScope.toastify('تم تعديل بيانات الموظف بنجاح', 1);
                    }).catch((rej) => console.log(rej))


            }
            else {
                $scope.changeRequest = {
                    EntityType: 'Employee', EntityId: $scope.updateEmployeeId, OldValue: null,
                    NewValue: $scope.employee, ChangedBy: $rootScope.LogedInUser.username
                }
                employeeService.addNewEmployee($scope.employee)
                    .then((res) => {
                        $rootScope.toastify('تم اضافة موظف بنجاح', 1);
                        $location.path('/editEmployee/' + res.data.id);
                    }).catch((rej) => console.log(rej))

            }
        }

      
    }


    var healthDz = null;

    // =====================
    // ADD MODE
    // =====================
    if (!$routeParams.employeeId) {
        initHealthInsuranceDropzone();
    }


    function initHealthInsuranceDropzone() {
        if (healthDz) return; // prevent double init

        Dropzone.autoDiscover = false;

        var dz = new Dropzone("#healthInsuranceDropzone", {
            url: "/Upload/UploadFile",
            paramName: "file",
            maxFiles: 1,
            addRemoveLinks: true,
            acceptedFiles: ".jpeg,.jpg,.png,.gif,.pdf,.doc,.msg",
            dictDefaultMessage: "ارفع ملف التأمين الصحي",

            init: function () {
                var dropzone = this;

                // =====================
                // Upload success
                // =====================
                dropzone.on("success", function (file, response) {
                    file.serverFileName = response.fileName;
                    $scope.$apply(function () {
                        $scope.employee.HealthInsuranceFile = response.fileName;
                    });
                });

                // =====================
                // Remove file
                // =====================
                dropzone.on("removedfile", function () {
                    $scope.$apply(function () {
                        $scope.employee.HealthInsuranceFile = "";
                    });
                });

                // =====================
                // Click → download
                // =====================
                dropzone.on("addedfile", function (file) {
                    file.previewElement.addEventListener("click", function (e) {
                        e.preventDefault();
                        e.stopPropagation();

                        var serverFileName =
                            file.serverFileName || $scope.employee.HealthInsuranceFile;

                        if (!serverFileName) return;

                        window.location.href =
                            "/Upload/Download?fileName=" +
                            encodeURIComponent(serverFileName);
                    });
                });

                // =====================
                // EDIT MODE (show existing file)
                // =====================
                if ($scope.employee.HealthInsuranceFile) {
                    loadExistingFile(dropzone, $scope.employee.HealthInsuranceFile);
                }
            }
        });

        function loadExistingFile(dropzone, serverFileName) {
            var mockFile = {
                name: serverFileName,
                size: 12345,
                accepted: true,
                serverFileName: serverFileName
            };

            dropzone.emit("addedfile", mockFile);
            dropzone.emit("complete", mockFile);

            // Show thumbnail for images
            if (/\.(jpg|jpeg|png|gif)$/i.test(serverFileName)) {
                dropzone.emit(
                    "thumbnail",
                    mockFile,
                    "/Uploads/" + serverFileName
                );
            }

            dropzone.files.push(mockFile);
        }
    }


  


    
})

app.controller('dashboardCtrl', function ($scope, employeeService, itemService, $location, $http, $timeout, $rootScope, NgTableParams) {

    $scope.planningData = [];

    // ng-table init
    $scope.planningTableParams = new NgTableParams({}, {
        dataset: $scope.planningData
    });
    $scope.planningTableParams.settings().counts = [];

    $scope.loadOverview = function () {
        $http.get('/Planning/Overview', {
            params: {
                fromYear: 2020,
                toYear: 2029
            }
        }).then(function (res) {

            $scope.planningData = res.data;

            // update table
            $scope.planningTableParams.settings({
                dataset: $scope.planningData
            });

            // draw chart
            drawChart(res.data);

        }, function () {
            alert("حدث خطأ أثناء تحميل بيانات التخطيط");
        });
    };

    function drawChart(data) {
        var years = data.map(x => x.Year);
        var planned = data.map(x => x.Planned);
        var issued = data.map(x => x.Issued);

        var ctx = document.getElementById('planningChart').getContext('2d');

        new Chart(ctx, {
            type: 'line',
            data: {
                labels: years,
                datasets: [
                    {
                        label: 'الاحتياج المخطط',
                        data: planned,
                        borderWidth: 3,
                        fill: false,
                        tension: 0.3
                    },
                    {
                        label: 'المصروف الفعلي',
                        data: issued,
                        borderDash: [5, 5],
                        borderWidth: 2,
                        fill: false,
                        tension: 0.3
                    }
                ]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: { position: 'bottom' }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: 'عدد المواد'
                        }
                    }
                }
            }
        });
    }

    $scope.selectedYear = null;

    $scope.detailsTableParams = new NgTableParams({}, {
        dataset: []
    });
    $scope.detailsTableParams.settings().counts = [];

    $scope.openYearDetails = function (year) {
        $scope.selectedYear = year;

        $http.get('/Planning/PlannedItemsByYear', {
            params: { year: year }
        }).then(function (res) {

            $scope.detailsTableParams.settings({
                dataset: res.data
            });

            var modal = new bootstrap.Modal(
                document.getElementById('planningDetailsModal')
            );
            modal.show();
        });
    };


    // load on page open
    $scope.loadOverview();

    $scope.IsAdmin = $rootScope.LogedInUser.userType == 0;

    $scope.loadNumbers = function () {
        employeeService.getNumberOfActiveEmployees().then((res) => {
            $scope.activeEmployeesCount = res.data;
        })
        employeeService.getNumberOfInactiveEmployees().then((res) => {
            $scope.inactiveEmployeesCount = res.data;
        })
        itemService.getNumberOfAllItems().then((res) => {
            $scope.itemsCount = res.data;
        })

        
    }
    $scope.loadNumbers();

    $scope.changeRequests = []

    $scope.loadChangeRequests = function () {
        $http.get('/ChangeRequest/GetAllChangeRequests').then((res) => {
            $scope.changeRequests = res.data;
            $scope.requestsTableParams.settings({ dataset: $scope.changeRequests } )
        })
    }
    $scope.loadChangeRequests();

    $scope.requestsTableParams = new NgTableParams(
        {
            page: 1,            // start on first page
            count: 10,          // items per page
            filter: {},
            sorting: { }// initial filter
        }
    );
    $scope.requestsTableParams.settings().counts = [];

    $scope.confirmAcceptRequest = function (requestId) {
        Swal.fire({
            title: 'هل انت متأكد من الموافقة على الطلب؟',
            text: "",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'نعم',
            cancelButtonText: 'لا'
        }).then((result) => {
            if (result.isConfirmed) $scope.acceptRequest(requestId);
        });
    }

    $scope.confirmRejectRequest = function (requestId) {
        Swal.fire({
            title: 'هل انت متأكد من رفض الطلب؟',
            text: "",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'نعم',
            cancelButtonText: 'لا'
        }).then((result) => {
            if (result.isConfirmed) $scope.rejectRequest(requestId);
        });
    }

    $scope.acceptRequest = function (requestId) {
       
        $http.post('/ChangeRequest/AcceptChangeRequest', { ChangeRequestId: requestId, ApprovedBy: $rootScope.LogedInUser.username }).then((res) => {
            $rootScope.toastify('تم الموافقة على الطلب وتعديل البيانات بنجاح', 1);
            $scope.loadChangeRequests();
        })
    }

    $scope.rejectRequest = function (requestId) {

        $http.post('/ChangeRequest/RejectChangeRequest', { ChangeRequestId: requestId, ApprovedBy: $rootScope.LogedInUser.username }).then((res) => {
            $rootScope.toastify('تم رفض طلب تعديل البيانات بنجاح', 1);
            $scope.loadChangeRequests();
        })
    }

    $scope.selectedRequest = {};
    $scope.differences = [];
    $scope.data = {};

    $scope.showDetails = function (request) {
        $scope.selectedRequest = request;
        $http.post('/ChangeRequest/GetDifferences', { ChangeRequestId: request.RequestId, EntityId: request.EntityId }).then((res) => {
            $scope.entityType = res.data.type;
            $scope.isAdd = res.data.IsAdd;

            if (res.data.IsAdd) {
                
                $scope.data = res.data.entity;
            } 
            if (!res.data.IsAdd && res.data.type == 'employee') {

                $scope.oldData = res.data.oldEntity;
                $scope.newData = res.data.newEntity;
            }
            if (res.data.IsAdd && res.data.type == 'item') {
                $scope.data = res.data.entity;
            }
            if (!res.data.IsAdd && res.data.type == 'item') {
                $scope.data = res.data.entity;
                $scope.itemReq = res.data.itemReq;
                
            }
            if (res.data.IsAdd && res.data.type == 'issuance') {
                $scope.data = res.data.entity;
                $scope.issuanceEmployee = res.data.employee;
                $scope.itemIssued = res.data.item;
                console.log($scope.data)
            }

           
                
        })
    }

  
})


app.controller('issuanceCtrl', function ($scope, employeeService, itemService, $location, $http, $timeout, $rootScope, NgTableParams, $routeParams) {
    $scope.loadIssuances = function () {
        $http.get('/Issuance/GetIssuancesByEmployeeId?EmployeeId=' + $routeParams.employeeId).then((res) => {
            $scope.issuances = res.data;

            $scope.issuancesTableParams.settings({ dataset: $scope.issuances });
        })
    }
    $scope.loadIssuances();

    $scope.loadEmployee = function () {
        employeeService.getEmployeeById($routeParams.employeeId).then((res) => {
            $scope.employee = res.data;
        })
    }
    $scope.loadEmployee();
    
    $scope.issuancesTableParams = new NgTableParams(
        {
            page: 1,            // start on first page
            count: 10,          // items per page
            filter: {},
            sorting: { Name: "asc" }// initial filter
        }
    );
    $scope.issuancesTableParams.settings().counts = [];
});

app.controller('entitlementCtrl',
    function ($scope, employeeService, itemService, $location, $http,
        $timeout, $rootScope, NgTableParams, $routeParams) {

        // =====================
        // Load data
        // =====================
        $scope.loadEntitlements = function () {
            $http.get('/Employee/GetEmployeeEntitlements?EmployeeId=' + $routeParams.employeeId)
                .then((res) => {
                    $scope.entitlements = res.data;
                    $scope.entitlementsTableParams.settings({ dataset: $scope.entitlements });
                });
        };

        $scope.loadEmployee = function () {
            employeeService.getEmployeeById($routeParams.employeeId).then((res) => {
                $scope.employee = res.data;
            });
        };

        $scope.entitlementsTableParams = new NgTableParams({
            page: 1,
            count: 10,
            filter: {},
            sorting: { Name: "asc" }
        });
        $scope.entitlementsTableParams.settings().counts = [];

        $scope.loadEmployee();
        $scope.loadEntitlements();

        // =====================
        // Issuance model
        // =====================
        $scope.issuance = {
            EmployeeId: null,
            ItemId: null,
            MatrixItemId: null,
            Quantity: 1,
            Type: 'Entitled',
            SignedReceiptPath: '',
            CreatedBy: $rootScope.LogedInUser.username,
            IssuanceDate: ''
        };

        // =====================
        // Select entitlement
        // =====================
        $scope.selectEntitlment = function (entitlement) {

            $scope.issuance = {
                EmployeeId: $scope.employee.EmployeeId,
                ItemId: entitlement.ItemId,
                MatrixItemId: entitlement.MatrixItemId,
                Quantity: 1,
                Type: 'Entitled',
                SignedReceiptPath: '',
                CreatedBy: $rootScope.LogedInUser.username,
                IssuanceDate: entitlement.EntitlementYear
            };

            $scope.selectedEntitlement = entitlement;

            // Open modal
            $('#issueModalForm').modal('show');
        };

        // =====================
        // Dropzone init / reset
        // =====================
        $scope.entitlementDropzone = null;

        $('#issueModalForm').on('shown.bs.modal', function () {
            if (!$scope.entitlementDropzone) {
                $scope.entitlementDropzone =
                    $rootScope.initFileDropzone(
                        "#entitlementReceiptDropzone",
                        $scope.issuance,
                        "SignedReceiptPath"
                    );
            }
        });

        $('#issueModalForm').on('hidden.bs.modal', function () {
            $scope.$apply(function () {

                // Clear Dropzone
                if ($scope.entitlementDropzone) {
                    $scope.entitlementDropzone.removeAllFiles(true);
                }

                // Clear model
                $scope.issuance.SignedReceiptPath = '';

                // Reset form
                if ($scope.entitlementForm) {
                    $scope.entitlementForm.$setPristine();
                    $scope.entitlementForm.$setUntouched();
                }
            });
        });

        $scope.changeRequest = {
            EntityType: 'Issuance', EntityId: '', OldValue: null,
            NewValue: '', ChangedBy: $rootScope.LogedInUser.username
        }

        $scope.addChangeRequest = function () {
            $scope.issuance.IssuanceDate =
                new Date(Date.UTC($scope.issuance.IssuanceDate, 0, 1));
            const { IssuanceId, ...issuanceRequest } = $scope.issuance;
            $scope.issuanceRequest = issuanceRequest;

            $scope.changeRequest.NewValue = JSON.stringify($scope.issuanceRequest);
            $http.post('/ChangeRequest/AddNewChangeRequest', { ChangeRequest: $scope.changeRequest }).then((res) => {
                $rootScope.toastify('تم حفظ طلب التعديل بنجاح, وسيتم مراجعته من الادارة', 1);

            })

        }

        // =====================
        // Submit issuance
        // =====================
        $scope.issueEntitlemet = function () {

           

            $scope.entitlementForm.$setSubmitted();

            if ($scope.entitlementForm.$invalid) {
                return;
            }

            $http.post('/Item/IsQuantityEnough', {
                Id: $scope.issuance.ItemId,
                Quantity: $scope.issuance.Quantity
            }).then((res) => {

                if (!res.data) {
                    $rootScope.toastify('الكمية غير متوفرة', 0);
                    return;
                }
                if ($rootScope.LogedInUser.userType != 0) {
                    $scope.addChangeRequest();

                } else {
                    $http.post('/Issuance/AddNewEntitledIssuance', { issuance: $scope.issuance })
                        .then(() => {
                            $rootScope.toastify('تم صرف المادة بنجاح', 1);
                            $('#issueModalForm').modal('hide');
                            $scope.loadEntitlements();
                        });
                }

             
            });
        };

        // =====================
        // Quantity helpers
        // =====================
        $scope.validateQuantity = function () {
            var max = $scope.selectedEntitlement.RemainingQty;
            if ($scope.issuance.Quantity < 1)
                $scope.issuance.Quantity = 1;
            else if ($scope.issuance.Quantity > max)
                $scope.issuance.Quantity = max;
        };

        $scope.blockTyping = function (event) {
            if (event.keyCode !== 38 && event.keyCode !== 40) {
                event.preventDefault();
            }
        };

    });



app.controller("itemLogCtrl", function ($scope, $http, NgTableParams, $routeParams,itemService) {

    $scope.itemLogs = [];

    $scope.itemLogsTableParams = new NgTableParams({}, {
        dataset: $scope.itemLogs
    });

    $scope.loadItem = function () {
        itemService.getItemById($routeParams.itemId).then((res) => {
            $scope.selectedItem = res.data;
        })
    }

    $scope.loadItemLogs = function () {

        $http.get("/ItemLog/ByItem?itemId=" + $routeParams.itemId)
            .then(function (res) {

            $scope.itemLogs = res.data;

            $scope.itemLogsTableParams.settings({
                dataset: $scope.itemLogs
            });

        }, function () {
            alert("حدث خطأ أثناء تحميل سجل الحركات");
        });
    };
    $scope.itemLogsTableParams.settings().counts = [];
    $scope.loadItemLogs();
    $scope.loadItem();
});
