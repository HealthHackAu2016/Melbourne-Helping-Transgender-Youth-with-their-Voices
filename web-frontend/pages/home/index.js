/**
 * React Static Boilerplate
 * https://github.com/kriasoft/react-static-boilerplate
 *
 * Copyright © 2015-present Kriasoft, LLC. All rights reserved.
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE.txt file in the root directory of this source tree.
 */

import React, { PropTypes } from 'react';
import Layout from '../../components/Layout';
import s from './styles.css';
import { title, html } from './index.md';
import history from '../../core/history'

class HomePage extends React.Component {
  constructor({ props }) {
    super()
    this.state = { recording: false }
  }

  componentDidMount() {
    document.title = 'transvoices'
    if (sessionStorage.getItem('target')) {
      history.push('/play')
    } else {
      history.push('/calibration')
    }
  }

  render() {
    return (
      <Layout className={s.content}>
        loading...
      </Layout>
    );
  }

}

export default HomePage;
