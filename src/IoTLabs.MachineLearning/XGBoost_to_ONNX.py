
#%%
try:
    import xgboost
except ImportError:
    !pip install xgboost
try:
    import onnxmltools
except ImportError:
    !pip install onnxmltools
from xgboost import XGBRegressor as xgbr
import matplotlib.pyplot as plt
get_ipython().run_line_magic('matplotlib', 'inline')
import seaborn as sns
sns.set()
import numpy as np
import pandas as pd
from sklearn.model_selection import train_test_split
from onnxmltools.convert import convert_xgboost
from onnxmltools.convert.common.data_types import FloatTensorType
from onnxmltools.utils import save_model


#%%
! dir
data = pd.read_csv("./PowerPlantDataSet.csv", index_col=None, header=None, skiprows=1)
data.head()

#%%
sns.heatmap(data.corr().abs())


#%%
for i in range(4):
    sns.jointplot(data[i],data[4], kind="kde")


#%%
X = data[[0,1,2,3]] # here we convert the old labels into numbers to make them compatible with the model exporter
y = data[4]


#%%
X.head()


#%%
y.head()


#%%
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)


#%%
X_test.head()


#%%
# Make a CSV backup of our test data without column names or indexes
test_data = X_test.copy()
test_data['target'] = y_test
test_data.to_csv("test_data.csv", header=False, index=False)


#%%
test_data.head()


#%%
model = xgbr()
model.fit(X_train, y_train, eval_set=[(X_test, y_test)])


#%%
conv_model = convert_xgboost(model, initial_types=[('float_input', FloatTensorType(shape=[1, 4]))])
assert(conv_model is not None)

save_model(conv_model, 'model.onnx')


